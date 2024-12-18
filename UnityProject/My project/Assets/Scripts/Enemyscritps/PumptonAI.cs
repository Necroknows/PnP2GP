using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

// * Pumpton the Pumpkin.
public class PumptonAI : MonoBehaviour, IDamage
{//Model / Prefab
    [SerializeField] Renderer model;
    [SerializeField] GameObject pumptonShot; //Prefab
    [SerializeField] Transform shootPOS;
    //HP & Dmg
    [SerializeField] int HP;
    [SerializeField] int maxHP;
    [SerializeField] int dmgToPlayer;
    [SerializeField] int dmgFromPlayer;
    //Shooting Parameters
    [SerializeField] float normalShootRange = 10f;
    [SerializeField] float chargedShotRange = 25f;
    [SerializeField] float minimumChargedShotRange = 10f;
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
    public Image healthBarForeground;


    // Start is called before the first frame update
    void Start()
    {
        HP = maxHP;
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, GameManager.instance.player.transform.position);
            faceTarget();

            if (canShoot && playerInRange)
            {
                if (distanceToPlayer <= normalShootRange)
                {
                    StartCoroutine(Shoot(1.0f));
                }
                else if (distanceToPlayer > normalShootRange && distanceToPlayer <= chargedShotRange)
                {
                    if (distanceToPlayer > minimumChargedShotRange)
                    {
                        StartCoroutine(beginChargedShot());
                    }
                }
            }
        }
        //Shoot Cooldown
        if (!canShoot)
        {
            shootTimer -= Time.deltaTime;
            if (shootTimer <= 0f)
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
            transform.rotation = Quaternion.Lerp(transform.rotation, rot * Quaternion.Euler(0, 90, 0), Time.deltaTime * rotateSpeed);
        }
        //Debugging...
        //Debug.DrawLine(transform.position, transform.position + directionToPlayer * 5, Color.red);
        //Debug.DrawLine(transform.position, transform.position + transform.forward * 5, Color.green);
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

        if (other.CompareTag("Player"))
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

    public void takeDamage(int damage)
    {
        // for an enemy to take any form of damage it has to also derive from IDamage and the spelling 
        // has to remain consistant ... i fixed it and moved your code from this section to the interface.
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
        if (!isChargingShot)//Check prevents the charged shot from spam firing.
        {
            isChargingShot = true;
            yield return new WaitForSeconds(chargedDuration);

            GameObject bullet = Instantiate(pumptonShot, shootPOS.position, shootPOS.rotation);
            Rigidbody bulletRb = bullet.GetComponent<Rigidbody>();

            if (bulletRb != null)
            {//Sets shoot direction.
                bulletRb.AddForce(Vector3.forward * chargedShotForce, ForceMode.Impulse);
            }

            yield return new WaitForSeconds(delayAfterAttack);
            //CD Reset
            canShoot = false;
            shootTimer = shootCD;
            isChargingShot = false;
        }
    }
    IEnumerator flashColor()
    {
        Material[] mats = model.materials;
        for (int model_iterator = 0; model_iterator < mats.Length; model_iterator++)
        {
            mats[model_iterator].color = Color.red;//Changes ALL materials to red via loop.
        }
        model.materials = mats;
        yield return new WaitForSeconds(0.1f);
        for (int model_iterator = 0; model_iterator < mats.Length; model_iterator++)
        {
            mats[model_iterator].color = colorOrig;//Reverts colors to original.
        }
        model.materials = mats;
    }

    public void takeDamage(int amount, Vector3 Dir)
    {
        HP -= amount;
        UpdateHealthBar();
        Debug.Log("Pumpton took damage! HP: " + HP);
        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            Debug.Log("Pumpton HP reached 0. Destroying Pumpton.");
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }


    private void UpdateHealthBar()
    {
        float healthPercent = (float)HP / maxHP;
        healthBarForeground.fillAmount = healthPercent;
    }
}