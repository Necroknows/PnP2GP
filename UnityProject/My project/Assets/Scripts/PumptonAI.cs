using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

//Lastest push -
// * Updated with range check to stop archer from firing arrows continuously.
// * Also modified for a stationary variant.
// * It has been decided that he is pumpkin. I'll rename later...
// * Pumpton the Pumpkin.
public class PumptonAI : MonoBehaviour
{//Model / Prefab
    [SerializeField] Renderer model;
    [SerializeField] GameObject pumptonShot; //Prefab
    [SerializeField] Transform shootPOS;
 //HP & Dmg
    [SerializeField] int HP;
    [SerializeField] int dmgToPlayer;
    [SerializeField] int dmgFromPlayer;
 //Shooting Parameters
    [SerializeField] float normalShootRange = 10f;
    [SerializeField] float chargedShotRange = 15f;
    [SerializeField] float chargedShotForce;
    [SerializeField] float chargedDuration;
    [SerializeField] float delayAfterAttack;
    [SerializeField] float shootCD;
    [SerializeField] int rotateSpeed;
    //Other
    private bool isChargingShot;
    private bool playerInRange;
    private bool canShoot = true;
    private Color colorOrig;
    private float shootTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
            faceTarget();

            if (canShoot)
            {
                if(distanceToPlayer <= normalShootRange)
                {
                    StartCoroutine(Shoot(1.0f));
                }
                else if(distanceToPlayer > normalShootRange && distanceToPlayer <= chargedShotRange)
                {
                    StartCoroutine(Shoot(chargedShotForce));
                }
            }
        }
        //Shoot Cooldown
        if(!canShoot)
        {
            shootTimer -= Time.deltaTime;
            if(shootTimer <= 0f)
            {
                canShoot = true;
            }    
        }
    }

    void faceTarget()
    {
        Vector3 directionToPlayer = (GameManager.instance.player.transform.position - transform.position).normalized;
        if (directionToPlayer != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(new Vector3(directionToPlayer.x, 0, directionToPlayer.z));
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
        }
            //Debugging...
        Debug.DrawLine(transform.position, transform.position + directionToPlayer * 5, Color.red);
        Debug.DrawLine(transform.position, transform.position + transform.forward * 5, Color.green);
    }

    void PumptonShot(float shootForce)
    {
        if (pumptonShot != null && shootPOS != null)
        {
            GameObject bullet = Instantiate(pumptonShot, shootPOS.position, shootPOS.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {
                bulletRb.AddForce(shootPOS.forward * shootForce, ForceMode.Impulse);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Projectile"))
        {
            takeDmg(dmgFromPlayer);
            //Destroys upon impact.
            Destroy(other.gameObject);
        }
        else if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDmg(int damage)
    {
        HP -= damage;
        Debug.Log("Pumpton took damage! HP: " + HP);
        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            Debug.Log("Pumpton HP reached 0. Destroying Pumpton.");
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator Shoot(float shootForce)
    {
        PumptonShot(shootForce);

        canShoot = false;
        shootTimer = shootCD;
        yield return new WaitForSeconds(shootCD);
        canShoot = true;
    }

    IEnumerator beginChargedShot()
    {
        isChargingShot = true;
        yield return new WaitForSeconds(chargedDuration);

        GameObject bullet = Instantiate(pumptonShot, shootPOS.position, shootPOS.rotation);
        Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(transform.forward * chargedShotForce, ForceMode.Impulse);

        yield return new WaitForSeconds(delayAfterAttack);
        //CD Reset
        canShoot = false;
        shootTimer = shootCD;
        isChargingShot = false;
    }
    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}