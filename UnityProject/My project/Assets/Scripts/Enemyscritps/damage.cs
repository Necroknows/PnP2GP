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
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, followReticle, stationary, chaser, arrow, lobber, laser, melee };  // Enum defining the various damage types

    // --- COMPONENT REFERENCES ---
    [SerializeField] damageType type;        // Current damage type of this object
    [SerializeField] Rigidbody rb;           // Rigidbody component for movement physics
    [SerializeField] ParticleSystem laserBullet;
    List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

    // --- DAMAGE PROPERTIES ---
    [SerializeField] int damageAmount;       // The amount of damage dealt
    [SerializeField] int speed;              // Speed at which the object moves
    [SerializeField] int destroyTime;        // Time before the object is destroyed
    [SerializeField] float damageInterval;   // Interval between stationary damage ticks
    [SerializeField] int force;              // Force applied for pushback effect
    public ParticleSystem hitEffect;         // hit effect to assign to bullets 
    // --- STATE TRACKING ---
    private bool isPlayerinField = false;    // Flag to track if the player is within the stationary damage field
    private Coroutine statDmgCoroutine;      // Reference to the stationary damage coroutine
    public bool isRichochet = false;         // to toggle destroy on collision defualt
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
        else if (type == damageType.followReticle)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        else if (type == damageType.laser)
        {
            laserBullet = this.GetComponent<ParticleSystem>();
            laserBullet.Play();
            Destroy(gameObject, destroyTime);
        }
    }

    // Method to handle collision with other objects
    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        if (hitEffect != null)
        {
            StartCoroutine(ShowHit());
       
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            // Stationary damage: start damage over time coroutine
            if (type == damageType.stationary)
            {
                isPlayerinField = true;
                statDmgCoroutine = StartCoroutine(DealStationaryDamage(dmg));

            }
            else if(type==damageType.melee)
            {

                // Apply damage and push force based on object type
                Vector3 targetPOS = GameManager.instance.player.transform.position;
               Vector3 playerDir= (targetPOS - transform.parent.position).normalized; 

                if (force>0)
                {
                    dmg.takeDamage(damageAmount, (playerDir * force));
                }
                else
                {
                    dmg.takeDamage(damageAmount,Vector3.zero);
                }
            }
            else
            {
                // Apply damage and push force based on object type
                Vector3 targetPOS = GameManager.instance.player.transform.position;
                Vector3 playerDir = (targetPOS - transform.position);

                if (force > 0)
                {
                    dmg.takeDamage(damageAmount, playerDir * force);
                }
                else
                {
                    dmg.takeDamage(damageAmount, Vector3.zero);
                }

                if (type == damageType.bullet || type == damageType.chaser || type == damageType.lobber || type == damageType.arrow)
                {
                    Destroy(gameObject);  // Destroy object after applying damage
                }
            }
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (hitEffect != null)
        {
            StartCoroutine(ShowHit());

        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            Vector3 targetPOS = GameManager.instance.player.transform.position;
            Vector3 playerDir = (targetPOS - transform.position);

            if (force > 0)
            {
                dmg.takeDamage(damageAmount, playerDir * force);
            }
            else
            {
                dmg.takeDamage(damageAmount, Vector3.zero);
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
    private IEnumerator ShowHit()
    {
       ParticleSystem hit= Instantiate(hitEffect, transform.position, Quaternion.identity);
        yield return new WaitForSeconds(0.25f);
        Destroy(hit );
    }

    // Update method to control chaser object movement towards the player
    private void Update()
    {
        if (type == damageType.chaser)
        {
            rb.velocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed;
            transform.LookAt(GameManager.instance.player.transform);
        }
        else if (type == damageType.followReticle)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Vector3 spot = ray.GetPoint(1000); // Default far point if nothing is hit
            if (Physics.Raycast(ray, out hit, 1100, LayerMask.GetMask("Player")))
            {
                spot = hit.point; // Update target point if something is hit
            }
            Vector3 targetDirection = spot - transform.position;
            rb.velocity = targetDirection.normalized * speed;
        }
        if (GameManager.instance.playerScript.getHP() <= 0 && statDmgCoroutine != null)
        {
            StopCoroutine(statDmgCoroutine);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!isRichochet)
        {
            Destroy(gameObject);
        }
    }
}


