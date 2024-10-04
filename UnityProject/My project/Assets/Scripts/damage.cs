using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, chaser};
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    // Start is called before the first frame update
    void Start()
    {   // standard bullets just travel in a straight line 
        if(type == damageType.bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        if(type==damageType.chaser)
        {
            Destroy(gameObject, destroyTime);
        }
    }

  private void OnTriggerEnter(Collider other)
    {
        if(other.isTrigger)
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if(dmg != null )
        {
            dmg.takeDamage(damageAmount);
        }
        else if(type==damageType.bullet||type==damageType.chaser)
        { 
        Destroy(gameObject);
        }
    }
    private void Update()
    {
        if(type==damageType.chaser)
        {                                // gets player postions           //bullet position   //always normalize to keep scales reliable
            rb.velocity = (GameManager.instance.player.transform.position - transform.position).normalized*speed*Time.deltaTime;
        }
    }
}
