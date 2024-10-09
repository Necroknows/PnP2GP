using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyGhostAI : MonoBehaviour, IDamage
{
    //nav mesh for ghost
    [SerializeField] NavMeshAgent agent;
    //ghost model
    [SerializeField] Renderer model;
    //shield obj for defense action
    [SerializeField] GameObject orbShield;
    //circling orbs that do damage
    [SerializeField] GameObject orbBullet;
    // transform for orbBullet to rotate around
   // [SerializeField] Transform orbAttackPos;
    //where the ghost can see from
    [SerializeField] Transform headPos;
    //[SerializeField] Animator animator;

    //ghost HP
    [SerializeField] int HP;
    //how many dashes to do in a sequence
    [SerializeField] int numOfDashes;
    //how long to wait between dashes in a sequence
    [SerializeField] int secBetweenDashes;
    //how much faster to make the ghost speed during dash
    [SerializeField] int dashSpeedMutliplier;
    //how long orbBullet attack lasts
    [SerializeField] int orbAttackTime;
    //how quickly the ghost rotates towards the player
    [SerializeField] int rotateSpeed;
    //how long a dash lasts
    [SerializeField] float dashTime;

    //gets player position
    Vector3 playerDir;
    
    //if the ghost attack/dash sequence
    bool isAttacking;
    //if orb shield active
    bool isOrbShieldActive;
    //if orb bullet active
    bool isOrbBulletActive;
    //if dashing/sheild sequence
    bool isDashing;
    //if player is in range
    bool playerInRange;

    Color colorOrig;
    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = GameManager.instance.player.transform.position;
        if (playerInRange)
        {
            //sets the destination of enemy to player 
            agent.SetDestination(GameManager.instance.player.transform.position);
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                //calls function to update rotation towards player
                faceTarget();
            }

            if (!isAttacking)
            {
                //coroutine to shoot when player is in range
                if (!isDashing && !isOrbBulletActive)
                {
                    //coroutine to shoot when player is in range
                    StartCoroutine(dash());
                    StartCoroutine(orbAttack());
                }

            }

        }
        if (!playerInRange)
        {
            if(isOrbShieldActive || isOrbBulletActive)
            {
                orbShield.gameObject.SetActive(false);
                orbBullet.gameObject.SetActive(false);
            }
            
        }
    }

    void faceTarget()
    {
        //rotates the enemy to the player smoothly
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);

    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
    
   
    IEnumerator dash()
    {
        isDashing = true;
        
        //grouping dashes together, for creating more dynamic dashes
        for (int currDashNum = 0; currDashNum < numOfDashes; currDashNum++)
        {
            //increases speed of ghost
            agent.speed *= dashSpeedMutliplier;
            // turns on orb sheild
            orbShieldActiveToggle();
            //determines time for dash
            yield return new WaitForSeconds(dashTime);
            //returns to original speed
            agent.speed /= dashSpeedMutliplier;
            //turns off orb sheild 
            orbShieldActiveToggle();
            //waits for time between the next dash in loop
            yield return new WaitForSeconds(secBetweenDashes);
        }
        isDashing = false;

    }
    public void orbShieldActiveToggle()
    //turns the orb sheild on and off
    {
            if (!isOrbShieldActive)
            {
                isOrbShieldActive = true;
                orbShield.gameObject.SetActive(true);
            }
            else if (isOrbShieldActive)
            {
                orbShield.gameObject.SetActive(false);
                isOrbShieldActive = false;
            }
    }

    IEnumerator orbAttack()
    {
        orbBulletActiveToggle();
        yield return new WaitForSeconds(orbAttackTime);
        orbBulletActiveToggle();

    }
    public void orbBulletActiveToggle()
    //turns the orb sheild on and off
    {
        if (!isOrbBulletActive)
        {
            isOrbShieldActive = true;
            orbBullet.gameObject.SetActive(true);
        }
        else if (isOrbBulletActive)
        {
            orbBullet.gameObject.SetActive(false);
            isOrbShieldActive = false;
        }
    }

    public void takeDamage(int amount)
    {
        //decreases health by damage amount
        HP -= amount;

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            //updates game manager game goal and destroys the enemy
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

}
