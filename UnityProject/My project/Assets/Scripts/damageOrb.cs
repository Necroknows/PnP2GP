using System;
using System.Collections;
using UnityEngine;


public class damageOrb : MonoBehaviour
{

    [SerializeField] int damageAmount;
    [SerializeField] float damageInterval; // Interval for stationary damage
    private bool isPlayerinField = false;
    private Coroutine statDmgCoroutine;
    private enemyGhostAI ghost;



    // Start is called before the first frame update
    void Start()
    {
       
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
                isPlayerinField = true;
                statDmgCoroutine = StartCoroutine(DealStationaryDamage(dmg));
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
        {
            return;
        }

        if (statDmgCoroutine != null)
        {
            isPlayerinField = false;
            StopCoroutine(statDmgCoroutine); // Stop dealing poison damage when the player exits
        }

    }

    private IEnumerator DealStationaryDamage(IDamage dmg)
    {
        while (isPlayerinField)
        {
            if (!ghost)
            {
                dmg.takeDamage(damageAmount);
            }
            yield return new WaitForSeconds(damageInterval); // Wait for the next damage tick
            if (!ghost)
            {
                dmg.takeDamage(damageAmount);
            }
        }
    }

    private void Update()
    {
        
    }
}
