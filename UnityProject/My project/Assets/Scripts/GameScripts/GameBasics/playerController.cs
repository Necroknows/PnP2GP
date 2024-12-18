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
    [Header("COMPONENT REFERENCES")]

    [SerializeField] CharacterController controller;  // Reference to the CharacterController for player movement
    [SerializeField] LayerMask ignoreMask;            // Mask to ignore certain layers during raycast
    [SerializeField] Transform shootPos;              // The position from which bullets are fired
    [SerializeField] GameObject bullet;               // Prefab for the bullet object
    [SerializeField] GameObject shieldVisual;

    [Header("PLAYER STATS AND MOVEMENT")]

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
    [SerializeField] float shieldRegenTime;  // Time before shield comes back
    //[SerializeField] float hoverSpeed;    // for item hover 
    //[SerializeField] float hoverRange;

    [Header("WEAPON STATS AND SHOOTING")]

    [SerializeField] List<GunStats> gunList = new List<GunStats>();
    [SerializeField] float shootRate;    // Rate of fire (time between shots)
    [SerializeField] int shootDist;      // Maximum shooting distance
    [SerializeField] Transform gunHolderTransform;  // Where the gun will be placed (e.g., player's hand)
    public GameObject gunModel;          // Store the current gun model
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] AudioSource gunShotNoise;      // Stores the sound for the gunshot

    [Header("INTERACTION / INVENTORY / QUESTS")]

    //[SerializeField] float interactionRange;
    public LayerMask interactionLayer;
    InventoryManager inventory;
    public KeyCode interactKey = KeyCode.E;
    [SerializeField] GameObject objectToRetrieve;
    public QuestLog QuestLog = null;

    [Header("DYNAMIC STATE VARIABLES")]

    Vector3 moveDir;      // Direction of player movement
    Vector3 playerVel;    // Player's velocity including vertical movement and push force
    public Vector3 pushDir; // Direction and force of any external push applied to the player
    public float Speed = 5f;

    [Header("MISC VARIABLES")]

    int HPOrig;           // Original player health at start
    int SelectGunPos;     // Hold the Current weapon postion in list for proper cycling 
    int jumpCount;        // Number of jumps the player has made
    /*bool isSprinting;*/     // Is the player currently sprinting          //// future use ... make sure to uncomment in Sprint()
    bool isShooting;      // Is the player currently shooting
    bool isjumping;       // Is the player currently jumping
    bool shieldUp; // to check if the shield is active
    bool canDash;

    public Transform carryPosition;
    private bool hasObject;


    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;           // Set original health value
        fuel = 0;                // sets starting fuel to 0 
        gunShotNoise.playOnAwake = false;   // Ensures audio does not play immediately, still make sure it is checked as false in audio component
        updatePlayerUI();      // Initialize player UI
        spawnPlayerAtStart();         // DropPlayer at SpawnPos
        QuestLog = this.AddComponent<QuestLog>();
        inventory = InventoryManager.instance;
        canDash = false;
        shieldUp = true;
    }
    public void spawnPlayer()
    {
        controller.enabled = false;
        transform.position = GameManager.instance.playerSpawnPOS.transform.position;
        controller.enabled = true;
        shieldUp = true;
        HP = HPOrig;
        fuel = 0;
        UIManager.Instance.UpdatePlayerHealthBar(HP);
        UIManager.Instance.UnpauseGame();

    }
    public void spawnPlayerAtStart()
    {
        controller.enabled = false;
        transform.position = GameManager.instance.playerStartPOS.transform.position;
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
        //Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        // Perform movement and sprinting logic if the game is not paused
        if (!UIManager.Instance.isPaused)
        {
            movement();
            selectGun();
            /*if (objectToRetrieve != null && Input.GetKeyDown(interactKey))
            { HandleInteraction(); }*/
        }
        sprint();
        //ItemBounce();
    }

    //Player Interaction
    /*private void HandleInteraction()
    {
        if (objectToRetrieve != null)
        {
            ItemPickup itemPickup = objectToRetrieve.GetComponent<ItemPickup>();

            if (itemPickup != null)
            {
                itemPickup.Pickup();        //call pickup() from ItemPickup
                Debug.Log("Object Retrieved " + objectToRetrieve.name);
            }
            objectToRetrieve = null;    //clear reference to object
        }
    }*/


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

        // Handle Dashing (if jump is active and fuel is available and canDash is true)
        if (isjumping && fuel > 0 && canDash)
        {
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                pushDir = (Input.GetAxis("Vertical") * transform.forward * (jumpSpeed * 2)) +
                          (Input.GetAxis("Horizontal") * transform.right * (jumpSpeed * 2)) +
                          new Vector3(0, jumpSpeed * 2, 0);  // Apply velocity for Dash
                fuel -= 1;   // Reduce fuel on Dash
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

            if (Physics.Raycast(ray, out hit, 1100, LayerMask.GetMask("Player")))
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
        if (shieldUp)
        {
            shieldUp = false;
            if (shieldVisual != null)
            {
                shieldVisual.SetActive(false);
                StartCoroutine(RegenShield());
            }
        }
        else
        {
            HP -= amount;
            updatePlayerUI();
            StartCoroutine(flashDamage());
        }
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

    public void DropAllItems()
    {
        if (inventory.Items.Count > 0)
        {
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                if (inventory.Items[i].itemID != inventory.recipeBookItemID)
                {
                    inventory.RemoveItem(inventory.Items[i]);
                    i--;
                }
            }
        }
    }

    IEnumerator RegenShield()
    {
        yield return new WaitForSeconds(shieldRegenTime);
        if (!shieldUp && shieldVisual != null)
        {
            shieldVisual.SetActive(true);
            shieldUp = true;
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

    public bool GetCanDash => canDash;

    public void SetCanDash(bool _canDash)
    {
        canDash = _canDash;
    }

    public void setHP(int amount)
    {
        HP += amount;
        if (HP > HPOrig)
        {
            HP = HPOrig;  // Prevent health from exceeding the original value
        }
    }

    public void setHPOrig(int amount)
    {
        HPOrig = amount;
    }

    public void setAmmo(int amount)
    {
        gunList[SelectGunPos].ammoCur += amount;
        if (gunList[SelectGunPos].ammoCur > gunList[SelectGunPos].ammoMax)
        {
            gunList[SelectGunPos].ammoCur = gunList[SelectGunPos].ammoMax;  // Prevent ammo from exceeding maximum capacity
        }
    }

    public void SetGuns(List<GunStats> guns)
    {
        gunList.AddRange(guns);
        WeaponSwap();
    }

    public void getGunStats(GunStats gun)
    {
        // Set the stats for shooting
        if (!gunList.Contains(gun))
        {
            gunList.Add(gun);

            SelectGunPos = gunList.Count - 1;
            shootRate = gun.shootRate;
            shootDist = gun.shootDist;
            bullet = gun.bullet;

            gunModel.GetComponent<MeshFilter>().sharedMesh = gun.gunModel.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gun.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        }
        else
        {
            SelectGunPos = gunList.IndexOf(gun);
            setAmmo(getAmmoMax());
        }


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
        if (gunList.Count > 0)
        {
            shootRate = gunList[SelectGunPos].shootRate;
            shootDist = gunList[SelectGunPos].shootDist;
            bullet = gunList[SelectGunPos].bullet;

            gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[SelectGunPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[SelectGunPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
        }
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

    internal void SetFuel(float fuelPickupAmount)
    {
        fuel += fuelPickupAmount;
        if (fuel > fuelmax)
        {
            fuel = fuelmax;
        }
    }

    public void ClearGunList()
    {
        gunList.Clear();
    }

    public int GetAmmoForGun(GunStats gun)
    {
        for(int eachGun = 0; eachGun < gunList.Count; eachGun++)
        {
            if (gunList[eachGun] == gun)
            {
                return gunList[eachGun].GetCurrAmmo();
            }
            
        }
        return 0;
    }

    public void SetAmmoForGun(GunStats gun)
    {
        for (int eachGun = 0; eachGun < gunList.Count; eachGun++)
        {
            if (gunList[eachGun] == gun)
            {
               gunList[eachGun].SetCurrAmmo(gun.GetCurrAmmo());
                break;
            }

        }
   
    }

    /*    private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Retrievable") && objectToRetrieve == null)
            {
                objectToRetrieve = other.gameObject;        //assigns detected object to objectToRetrieve
                Debug.Log("Object Entered: " + objectToRetrieve.name);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Retrievable") && objectToRetrieve == other.gameObject)
            {
                Debug.Log("Object Exited " + objectToRetrieve.name);
                objectToRetrieve = null;    //resets oTR to null if player leaves range
            }
        }*/

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