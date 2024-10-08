using System.Collections;
using UnityEngine;

public class damage : MonoBehaviour
{
    [SerializeField] enum damageType { bullet, stationary, chaser, arrow, ghostOrb};

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    // Start is called before the first frame update
    void Start()
    {   // standard bullets just travel in a straight line 
        if (type == damageType.bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        if (type == damageType.chaser)
        {
            Destroy(gameObject, destroyTime);
        }

        if(type==damageType.arrow)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }

        if (type == damageType.ghostOrb)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
        if (type == damageType.ghostOrb)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damageAmount);
        }
        else if(type==damageType.bullet||type==damageType.chaser || type == damageType.arrow)
        { 
        Destroy(gameObject);
        }
        else if(type == damageType.arrow) //Added arrow physics to allow for arrow sticking.
        {
            rb.isKinematic = true; //Stops physics
            transform.parent = other.transform;
            Destroy(gameObject, destroyTime);
        }
    }
    private void Update()
    {
        if (type == damageType.chaser)
        {                                // gets player postions           //bullet position   //always normalize to keep scales reliable
            rb.velocity = (GameManager.instance.player.transform.position - transform.position).normalized * speed * Time.deltaTime;
        }
    }

   
}
