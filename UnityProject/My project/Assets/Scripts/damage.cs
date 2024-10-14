using System.Collections;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, chaser, arrow, lobber };

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    [SerializeField] float damageInterval; // Interval for stationary damage
    private bool isPlayerinField = false;
    private Coroutine statDmgCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        if(!rb) rb = GetComponent<Rigidbody>();

        if (type == damageType.bullet || type == damageType.arrow)
        {
            rb.velocity = transform.forward * speed;
          
                Destroy(gameObject, destroyTime);
            
        }
        else if (type == damageType.chaser)
        {
            
                Destroy(gameObject, destroyTime);
            
        }
        else if (type==damageType.lobber)
        {
          // get the player location 
          Vector3 targetPOS=GameManager.instance.player.transform.position;

            // get the distance to the player 
            Vector3 playerDir = targetPOS-transform.position;
            float distToPlayer = playerDir.magnitude;

            // calculate the time it will take to get to that position
            float flightTime = distToPlayer / speed;

            // calculate the vertical velocity needed to get there accounting for gravity . 
            float vertVelo = (0.5f * Mathf.Abs(Physics.gravity.y) * flightTime);

            // combine out and up velocity
            Vector3 lobberVelo = (playerDir.normalized*speed)+Vector3.up*vertVelo;

            rb.velocity=lobberVelo;
            Destroy(gameObject, destroyTime);


        }

      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            if (type == damageType.stationary)
            {
                isPlayerinField = true;
                statDmgCoroutine = StartCoroutine(DealStationaryDamage(dmg));
            }
            else
            {
                dmg.takeDamage(damageAmount);
                if (type == damageType.bullet || type == damageType.chaser)
                {
                        Destroy(gameObject);
                    
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger) return;
    
        if (type == damageType.stationary && statDmgCoroutine != null)
        {
            isPlayerinField = false;
            StopCoroutine(statDmgCoroutine); // Stop dealing poison damage when the player exits
        }

    }

    private IEnumerator DealStationaryDamage(IDamage dmg)
    {
        while (isPlayerinField)
        {
            dmg.takeDamage(damageAmount);
            yield return new WaitForSeconds(damageInterval); // Wait for the next damage tick
            
        }
    }

    private void Update()
    {
        if (type == damageType.chaser)
        {
            rb.velocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed;
            transform.LookAt(GameManager.instance.player.transform);
        }
    }
}

