/*
 * Author(s): Alexandria Dixon, Aaron Goodwin, Jesse Mercer, Orion White
 * Date: 10-16-2024
 * Course: Full Sail University - Game Development Program
 * Project: Project and Portfolio 2
 * Description: 
 *     This script manages the player's movement, shooting mechanics, jetpack flight, damage interactions, and push force mechanic.
 *     It includes functionality for sprinting, jumping, and handling player inputs.
 *     Updates both health and fuel in the player UI.
 *
 * Version: 1.5 (Merged Mercer Personal and Main Project Code 10-17-2024)
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerController : MonoBehaviour, IDamage
{
    // --- COMPONENT REFERENCES ---
    [SerializeField] CharacterController controller;  // Reference to the CharacterController for player movement
    [SerializeField] LayerMask ignoreMask;            // Mask to ignore certain layers during raycast
    [SerializeField] Transform shootPos;              // The position from which bullets are fired
    [SerializeField] GameObject bullet;               // Prefab for the bullet object

    // --- PLAYER STATS AND MOVEMENT ---
    [SerializeField] int speed;          // Player movement speed
    [SerializeField] int sprintMod;      // Speed multiplier when sprinting
    [SerializeField] int jumpMax;        // Maximum number of jumps allowed
    [SerializeField] int jumpSpeed;      // Vertical velocity for jumping
    [SerializeField] int gravity;        // Gravity force affecting the player
    [SerializeField] int pushtime;       // Time duration for push effect to decay
    [SerializeField] int HP;             // Player's health points
    [SerializeField] int Ammo;           // Current ammo count
    [SerializeField] int AmmoMax;        // Maximum ammo capacity
    [SerializeField] float fuel;         // Jetpack fuel amount
    [SerializeField] float fuelmax;        // Maximum fuel amount for jetpack
    //[SerializeField] float hoverSpeed;    // for item hover 
    //[SerializeField] float hoverRange;
    // --- WEAPON STATS AND SHOOTING ---
    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] float shootRate;    // Rate of fire (time between shots)
    [SerializeField] int shootDist;      // Maximum shooting distance
    [SerializeField] Transform gunHolderTransform;  // Where the gun will be placed (e.g., player's hand)
    public GameObject gunModel;          // Store the current gun model
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] AudioSource gunShotNoise;      // Stores the sound for the gunshot

    // --- INTERACTION & INVENTORY ---
    [SerializeField] float interactionRange;
    public LayerMask interactionLayer;
    Inventory inventory = Inventory.instance;

    // --- DYNAMIC STATE VARIABLES ---
    Vector3 moveDir;      // Direction of player movement
    Vector3 playerVel;    // Player's velocity including vertical movement and push force
    public Vector3 pushDir; // Direction and force of any external push applied to the player

    // --- MISC VARIABLES ---
    int HPOrig;           // Original player health at start
    int SelectGunPos;     // Hold the Current weapon postion in list for proper cycling 
    int jumpCount;        // Number of jumps the player has made
    /*bool isSprinting;*/     // Is the player currently sprinting          //// future use ... make sure to uncomment in Sprint()
    bool isShooting;      // Is the player currently shooting
    bool isjumping;       // Is the player currently jumping
    [SerializeField] GameObject objectToRetrieve;
    public Transform carryPosition;
    private bool hasObject;


    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;           // Set original health value
        fuel = 0;                // sets starting fuel to 0 
        gunShotNoise.playOnAwake = false;   // Ensures audio does not play immediately, still make sure it is checked as false in audio component
        updatePlayerUI();      // Initialize player UI
        spawnPlayer();         // DropPlayer at SpawnPos
        inventory = Inventory.instance; // Get the inventory instance
    }
    public void spawnPlayer()
    {
        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawnPOS.transform.position;
        controller.enabled = true;
        HP = HPOrig;
        fuel = 0;
        UIManager.Instance.UpdatePlayerHealthBar(HP);
        UIManager.Instance.UnpauseGame();

    }

    // Update is called once per frame
    void Update()
    {
        // Draw a debug ray to visualize shooting direction
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        
        // Perform movement and sprinting logic if the game is not paused
        if (!UIManager.Instance.isPaused)
        {
            movement();
            selectGun();
            HandleInteraction();
        }
        sprint();
        //ItemBounce();

    }

    //Player Interaction
    private void HandleInteraction()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, interactionRange, interactionLayer))
            {
                //check if hit object has pickups component
                Pickups pickup = hit.collider.GetComponent<Pickups>();
                if(pickup != null)
                {
                    //call interact method
                    pickup.Interact();
                }

            }
        }
    }

    InventoryItem GetSelectedHerb()
    {
        return Inventory.instance.GetItems().Find(item => item.itemType == InventoryItem.ItemType.Herb);
    }

    void movement()
    {
        // Gradually reduce push force over time using Lerp
        if (pushDir != Vector3.zero)
        {
            pushDir = Vector3.Lerp(pushDir, Vector3.zero, pushtime * Time.deltaTime);
        }

        // Ground check to reset vertical velocity and jump logic
        if (controller.isGrounded)
        {
            jumpCount = 0;          // Reset jump count
            playerVel.y = 0f;       // Reset only vertical velocity when grounded
            isjumping = false;      // Player is no longer jumping
        }

        // Get movement direction based on player input
        moveDir = Input.GetAxis("Vertical") * transform.forward +
                  Input.GetAxis("Horizontal") * transform.right;

        // Apply movement (without push) based on speed
        controller.Move(moveDir * speed * Time.deltaTime);

        // Handle jump logic
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;            // Increase jump count
            playerVel.y = jumpSpeed; // Set vertical velocity for jump
            isjumping = true;       // Mark player as jumping
        }

        // Apply gravity to player velocity
        playerVel.y -= gravity * Time.deltaTime;

        // Handle flight (if jump is active and fuel is available)
        if (isjumping && fuel > 0)
        {
            if (Input.GetKey(KeyCode.F))
            {
                playerVel.y = (jumpSpeed/2)+2;  // Apply vertical velocity for flight
                fuel -= Time.deltaTime;   // Reduce fuel during flight
                updatePlayerUI();         // Update UI with remaining fuel
            }

            // Apply gravity for flight as well
            // playerVel.y -= gravity * Time.deltaTime;
        }

        // Apply the push mechanic to player movement
        controller.Move((playerVel + pushDir) * Time.deltaTime);

        // Call shoot logic when the player presses the shoot button
        if (Input.GetButton("Shoot") && gunList.Count > 0 && gunList[SelectGunPos].ammoCur > 0 && !isShooting && !hasObject)
        {
            StartCoroutine(shoot());
        }
    }

    // Sprinting logic
    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;      // Multiply speed for sprinting
            //isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;      // Reset speed after sprinting
            //isSprinting = false;
        }
    }

    // Shooting logic using a coroutine to manage shooting rate
    IEnumerator shoot()
    {
        if (SelectGunPos >= 0 && SelectGunPos < gunList.Count && gunList[SelectGunPos].ammoCur > 0)
        {
            StartCoroutine(flashMuzzle());
            isShooting = true;
            //reduce ammo count
            gunList[SelectGunPos].ammoCur--;

            // Calculate the target point from the reticle using raycasting
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            RaycastHit hit;
            Vector3 targetPoint = ray.GetPoint(1000); // Default far point if nothing is hit

            if (Physics.Raycast(ray, out hit))
            {
                targetPoint = hit.point; // Update target point if something is hit
            }

            // Calculate the direction from the shoot position to the target point
            Vector3 direction = (targetPoint - shootPos.position).normalized;
            Quaternion bulletRotation = Quaternion.LookRotation(direction);

            // Instantiate bullet at the shoot position aiming towards the target point
            Instantiate(bullet, shootPos.position, bulletRotation);
            // Plays the Audio for the Bullets when fired
            StartCoroutine(GunShot());
            // Wait for the rate of fire before enabling shooting again
            yield return new WaitForSeconds(shootRate);

            isShooting = false;
        }
        else
        {
            // Optionally handle what happens if ammo is 0 (play click sound, show reload prompt, etc.)
            yield return null; // Just end the coroutine if there's no ammo
        }
    }

    // Handles the gunshot audio
    IEnumerator GunShot()
    {
        //play gunshot from gunstats
        if (gunShotNoise != null && gunList[SelectGunPos].shootSound.Length > 0)
        {
            gunShotNoise.clip = gunList[SelectGunPos].shootSound[0];
            gunShotNoise.volume = gunList[SelectGunPos].shootVol;
            gunShotNoise.loop = false;
            gunShotNoise.Play();
        }
        yield return null;
    }

    // Handle player taking damage and apply a push force
    public void takeDamage(int amount, Vector3 Dir)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());

        // Apply the push force to the player's velocity
        if (Dir != Vector3.zero)
        {
            pushDir = Dir;  // Receive push force from the damage script
        }

        if (HP <= 0)
        {
            UIManager.Instance.ShowLoseScreen();  // Handle player death
        }
    }

    // Flash red when the player takes damage
    IEnumerator flashDamage()
    {
        GameManager.instance.flashDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        GameManager.instance.flashDamageScreen.SetActive(false);
    }
    IEnumerator flashMuzzle()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        muzzleFlash.SetActive(false);
    }

    // Update player health and fuel UI elements
    public void updatePlayerUI()
    {
        UIManager.Instance.playerHpBar.fillAmount = (float)HP / HPOrig;
        UIManager.Instance.playerFuelBar.fillAmount = (float)fuel / fuelmax;
    }

    // Getters and setters for health, ammo, and other player stats
    public int getHP()
    {
        return HP;
    }

    public int getHPOrig()
    {
        return HPOrig;
    }

    public int GetAmmo()
    {
        return (SelectGunPos >= 0 && SelectGunPos < gunList.Count) ? gunList[SelectGunPos].ammoCur : 0;
    }

    public int getAmmoMax()
    {
        return (SelectGunPos >= 0 && SelectGunPos < gunList.Count) ? gunList[SelectGunPos].ammoMax : 0;
    }

    public void setHP(int amount)
    {
        HP += amount;
        if (HP > HPOrig)
        {
            HP = HPOrig;  // Prevent health from exceeding the original value
        }
    }

    public void setAmmo(int amount)
    {
        gunList[SelectGunPos].ammoCur += amount;
        if (gunList[SelectGunPos].ammoCur > gunList[SelectGunPos].ammoMax)
        {
            gunList[SelectGunPos].ammoCur = gunList[SelectGunPos].ammoMax;  // Prevent ammo from exceeding maximum capacity
        }
    }
    public void getGunStats(GunStats gun)
    {
        // Set the stats for shooting
        gunList.Add(gun);

        SelectGunPos = gunList.Count - 1;
        shootRate = gun.shootRate;
        shootDist = gun.shootDist;
        bullet = gun.bullet;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;


    }
    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && SelectGunPos < gunList.Count - 1)
        {
            SelectGunPos++;
            WeaponSwap();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && SelectGunPos > 0)
        {
            SelectGunPos--;
            WeaponSwap();
        }
    }
    void WeaponSwap()
    {

        shootRate = gunList[SelectGunPos].shootRate;
        shootDist = gunList[SelectGunPos].shootDist;
        bullet = gunList[SelectGunPos].bullet;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[SelectGunPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[SelectGunPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
    public List<GunStats> GetGuns()
    {
        return gunList;
    }
    public GunStats GetGunCurr()
    {
        if (gunList.Count > 0)
        {
            return gunList[SelectGunPos];
        }
        else return null;
    }

    internal float GetFuel()
    {
        return fuel;
    }

    internal float GetFuelMax()
    {
        return fuelmax;
    }

    internal void SetFuel(int fuelPickupAmount)
    {
        fuel += fuelPickupAmount;
        if (fuel > fuelmax)
        {
            fuel = fuelmax;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Retrievable") && !hasObject&& other.transform.parent==null)
        {
            objectToRetrieve = other.gameObject;        //assigns detected object to objectToRetrieve
            Debug.Log("Object Entered: " + objectToRetrieve.name);
            PickUpObject();                             //automatically picks up object
        }
        else if (other.CompareTag("Drop Off") && hasObject)
        {
            GameManager.instance.updateMiniGoal(1);
            DropOffObject();
        }
    }
    //called to pick up object
    void PickUpObject()
    {
        if (objectToRetrieve != null && !hasObject)
        {
            objectToRetrieve.transform.SetParent(transform);        //parents object to seeker
            objectToRetrieve.transform.localPosition = carryPosition.transform.localPosition; //sets carry position
            hasObject = true;                                       //updates carry status
            Debug.Log("Object Retrieved" + objectToRetrieve.name);
        }
    }

    //called to drop off object
    void DropOffObject()
    {
        if (objectToRetrieve != null)
        {

            objectToRetrieve.transform.SetParent(null);             //unparent object
            Destroy(objectToRetrieve);                              //destroy object
            objectToRetrieve = null;                                //clear reference to object
            hasObject = false;                                      //update carrying status

            //we'll either update game goals for the enemy in game manager or UI manager
            //Waiting on Jesse to confirm which script to use
            //gameManager.instance.UpdateGameGoal();
            Debug.Log("Object Dropped Off");
        }
    }
    //IEnumerator ItemBounce()
    //{
    //    //gunHolderTransform
    //   Vector3 startPos = gunModel.transform.position;
    //    // Hovering animation logic
    //    float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * (hoverRange / 2);
    //    transform.position = new Vector3(startPos.x, newY, startPos.z);
    //    yield return null;
    //}
}