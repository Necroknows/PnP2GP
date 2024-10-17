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

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

public class playerController : MonoBehaviour, IDamage
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
    [SerializeField] float fuelmax;      // Maximum jetpack fuel amount

    // --- WEAPON STATS AND SHOOTING ---
    [SerializeField] int shootDamage;    // Damage dealt by bullets
    [SerializeField] float shootRate;    // Rate of fire (time between shots)
    [SerializeField] int shootDist;      // Maximum shooting distance

    // --- DYNAMIC STATE VARIABLES ---
    Vector3 moveDir;      // Direction of player movement
    Vector3 playerVel;    // Player's velocity including vertical movement and push force
    public Vector3 pushDir; // Direction and force of any external push applied to the player

    // --- MISC VARIABLES ---
    int HPOrig;           // Original player health at start
    int jumpCount;        // Number of jumps the player has made
    bool isSprinting;     // Is the player currently sprinting
    bool isShooting;      // Is the player currently shooting
    bool isjumping;       // Is the player currently jumping

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;           // Set original health value
        fuelmax = fuel;        // Set maximum fuel value
        updatePlayerUI();      // Initialize player UI
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
        }
        sprint();
    }

    // Handles player movement, including jumping, jetpack flight, and applying push force
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
                playerVel.y = jumpSpeed;  // Apply vertical velocity for flight
                fuel -= Time.deltaTime;   // Reduce fuel during flight
                updatePlayerUI();         // Update UI with remaining fuel
            }
        }

        // Apply the push mechanic to player movement
        controller.Move((playerVel + pushDir) * Time.deltaTime);

        // Call shoot logic when the player presses the shoot button
        if (Input.GetButton("Shoot") && !isShooting)
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
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;      // Reset speed after sprinting
            isSprinting = false;
        }
    }

    // Shooting logic using a coroutine to manage shooting rate
    IEnumerator shoot()
    {
        if (getAmmo() > 0)
        {
            isShooting = true;

            // Use the camera's forward direction for bullet rotation
            Quaternion bulletRotation = Quaternion.LookRotation(Camera.main.transform.forward);

            // Instantiate bullet with corrected rotation
            Instantiate(bullet, shootPos.position, bulletRotation);

            setAmmo(-1);  // Reduce ammo count
        }
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
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

    // Update player health and fuel UI elements
    public void updatePlayerUI()
    {
        GameManager.instance.playerHpBar.fillAmount = (float)HP / HPOrig;
        GameManager.instance.playerFuelBar.fillAmount = (float)fuel / fuelmax;
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

    public int getAmmo()
    {
        return Ammo;
    }

    public int getAmmoMax()
    {
        return AmmoMax;
    }

    // Getter and Setter for fuel
    public float getFuel()
    {
        return fuel;
    }

    public void setFuel(float amount)
    {
        fuel += amount;
        if (fuel > fuelmax)
        {
            fuel = fuelmax;  // Prevent fuel from exceeding the maximum
        }
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
        Ammo += amount;
        if (Ammo > AmmoMax)
        {
            Ammo = AmmoMax;  // Prevent ammo from exceeding maximum capacity
        }
    }
}
