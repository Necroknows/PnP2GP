using System.Collections;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, chaser, arrow, };

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
        if (type == damageType.bullet || type == damageType.arrow)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        else if (type == damageType.chaser)
        {
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
            dmg.takeDamage(damageAmount);
        }
    }

    private void Update()
    {
        if (type == damageType.chaser)
        {
            rb.velocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }
}

