/*
 * Author(s): Alexandria Dixon, Duran Perry, Jesse Mercer
 * Date: 10-17-2024
 * Course: Full Sail University - Game Development Program
 * Project: Project and Portfolio 2
 * Description: 
 *     This script manages different types of damage objects, including bullets, chasers, arrows, stationary fields, and lobber projectiles.
 *     It handles damage application, velocity calculations, and interactions with the player (e.g., push force, damage over time).
 *     Special attention is given to gravity-based projectile arcs (lobbers) and continuous damage for stationary fields.
 *
 * Version: 1.5 (Merged Mercer Personal and Main Project 10-17-2024)
 * 
 * Additional Notes:
 * 
 */

using System.Collections;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, chaser, arrow, lobber };  // Enum defining the various damage types

    // --- COMPONENT REFERENCES ---
    [SerializeField] damageType type;        // Current damage type of this object
    [SerializeField] Rigidbody rb;           // Rigidbody component for movement physics

    // --- DAMAGE PROPERTIES ---
    [SerializeField] int damageAmount;       // The amount of damage dealt
    [SerializeField] int speed;              // Speed at which the object moves
    [SerializeField] int destroyTime;        // Time before the object is destroyed
    [SerializeField] float damageInterval;   // Interval between stationary damage ticks
    [SerializeField] int force;              // Force applied for pushback effect

    // --- STATE TRACKING ---
    private bool isPlayerinField = false;    // Flag to track if the player is within the stationary damage field
    private Coroutine statDmgCoroutine;      // Reference to the stationary damage coroutine

    // Start is called before the first frame update
    void Start()
    {
        // Ensure Rigidbody is assigned, or fetch from the GameObject
        if (!rb) rb = GetComponent<Rigidbody>();

        // Handle bullet and arrow logic: apply velocity and destroy after time
        if (type == damageType.bullet || type == damageType.arrow)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        // Handle chaser logic: destroy after time (updated in Update method)
        else if (type == damageType.chaser)
        {
            Destroy(gameObject, destroyTime);
        }
        // Handle lobber logic: calculate velocity for gravity-based projectile arcs
        else if (type == damageType.lobber)
        {
            Vector3 targetPOS = GameManager.instance.player.transform.position;
            Vector3 playerDir = targetPOS - transform.position;
            float distToPlayer = playerDir.magnitude;

            float flightTime = distToPlayer / speed;
            float vertVelo = (0.5f * Mathf.Abs(Physics.gravity.y) * flightTime);

            Vector3 lobberVelo = (playerDir.normalized * speed) + Vector3.up * vertVelo;

            rb.velocity = lobberVelo;
            Destroy(gameObject, destroyTime);
        }
    }

    // Method to handle collision with other objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            // Stationary damage: start damage over time coroutine
            if (type == damageType.stationary)
            {
                isPlayerinField = true;
                statDmgCoroutine = StartCoroutine(DealStationaryDamage(dmg));
            }
            else
            {
                // Apply damage and push force based on object type
                Vector3 targetPOS = GameManager.instance.player.transform.position;
                Vector3 playerDir = (targetPOS - transform.position).normalized;

                if (force > 0)
                {
                    dmg.takeDamage(damageAmount, playerDir * force);
                }
                else
                {
                    dmg.takeDamage(damageAmount, Vector3.zero);
                }

                if (type == damageType.bullet || type == damageType.chaser || type == damageType.lobber)
                {
                    Destroy(gameObject);  // Destroy object after applying damage
                }
            }
        }
    }

    // Stop stationary damage when player exits the damage field
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;

        if (type == damageType.stationary && statDmgCoroutine != null)
        {
            isPlayerinField = false;
            StopCoroutine(statDmgCoroutine);  // Stop stationary damage coroutine
        }
    }

    // Coroutine for dealing stationary damage over time
    private IEnumerator DealStationaryDamage(IDamage dmg)
    {
        while (isPlayerinField)
        {
            dmg.takeDamage(damageAmount, Vector3.up * force);  // Apply damage and push upwards
            yield return new WaitForSeconds(damageInterval);  // Wait for the next tick
        }
    }

    // Update method to control chaser object movement towards the player
    private void Update()
    {
        if (type == damageType.chaser)
        {
            rb.velocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed;
            transform.LookAt(GameManager.instance.player.transform);
        }
    }
}


