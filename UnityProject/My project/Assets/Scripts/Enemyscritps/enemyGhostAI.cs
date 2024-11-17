using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyGhostAI : MonoBehaviour, IDamage
{
    //nav mesh for ghost
    [SerializeField] NavMeshAgent agent;
    //animations for ghost
    [SerializeField] Animator animator;
    //ghost model
    [SerializeField] Renderer model;
    //model for prefab to replace it
    [SerializeField] GameObject modelSwitch;
    [SerializeField] bool isUsingNewModel;
    //shield obj for defense action
    [SerializeField] GameObject orbShield;
    //circling orbs that do damage
    [SerializeField] GameObject orbBullet;
    // transform for orbBullet to rotate around
    [SerializeField] Transform orbAttackPos;
    //where the ghost can see from
    [SerializeField] Transform headPos;
    //[SerializeField] Animator animator;

    //ghost HP
    [SerializeField] int HP;
    //how many dashes to do in a sequence
    [SerializeField] int numOfShield;
    //how long to wait between dashes in a sequence
    [SerializeField] int secBetweenShield;
    //how much faster to make the ghost speed during dash
    [SerializeField] int dashSpeedMutliplier;
    //how long orbBullet attack lasts
    [SerializeField] int orbAttackTime;
    //how long before next dash starts
    [SerializeField] float delayAfterAttack;
    //how quickly the ghost rotates towards the player
    [SerializeField] int rotateSpeed;
    //how long a dash lasts
    [SerializeField] float shieldTime;
    [SerializeField] int roamDist;//distance enemy roams
    [SerializeField] int roamPauseTime;//how long enemy stops at that location
    [SerializeField] float angleToPlayer;
    [SerializeField] int viewAngle; //view angle

    // Vector3 bulletPosVec;
    Vector3 startingPoint; //used for roam feature 
    //gets player position
    Vector3 playerDir;
    //gets the position and rotation of an object
    Vector3 modelSwitchPos;
    Quaternion modelSwitchRot;
    // Vector3 startingPos;

    //Spawn Effects
    public GameObject spawnEffectPrefab;
    public Vector3 effectOffset = new Vector3(0, 0, 0);

    //used to replace stopping distance for roaming vs going to player
    float stoppingDisOrig;
    //if the ghost attack/dash sequence
    bool isAttacking;
    //if dashing/sheild sequence
    bool isDashing;
    //if player is in range
    bool playerInRange;
    bool isRoaming;
    bool isDying;

    Color colorOrig;
    Coroutine someCo; //sets the coroutine to current coroutine
    // Start is called before the first frame update
    void Start()
    {
        SpawnEffect();
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);
        startingPoint = transform.position;
        stoppingDisOrig = agent.stoppingDistance;
        
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetFloat("Speed", agent.velocity.normalized.magnitude);

        
        
        if (playerInRange && !canSeePlayer())
        {

            //this calls roam if the enemy is not roaming
            //and if the remaining distance is very small
            if (agent.remainingDistance < 0.05f && !isRoaming)
            {
                someCo = StartCoroutine(roam());
            }

        }
        else if (!playerInRange)
        {
            agent.stoppingDistance = 0f;
            //this calls roam if the enemy is not roaming
            //and if the remaining distance is very small
            if (agent.remainingDistance < 0.05f && !isRoaming)
            {
                someCo = StartCoroutine(roam());
            }
        }
    }
    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDir.x, 0, playerDir.z), transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (!isAttacking)
                {
                    if (!isDashing)
                    {
                        //coroutine to shoot when player is in range
                        StartCoroutine(beginAttack());
                    }
                }
                //if (playerInRange)
                
                    //resets stopping distance from roaming
                    agent.stoppingDistance = stoppingDisOrig;
                

                return true;
            }
            
        }
        //catch to make sure stopping distance is 0 when player isnt being seen
        agent.stoppingDistance = 0;
        return false;
    }

    void faceTarget()
    {
        //rotates the enemy to the player smoothly
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);

    }
    IEnumerator roam()
    {
        isRoaming = true;
        agent.stoppingDistance = 0;
        yield return new WaitForSeconds(roamPauseTime);

        agent.stoppingDistance = 0;
        //sets makes the max distance to roam within a sphere and selects 
        //a random position
        
        Vector3 randomPos = Random.insideUnitSphere * roamDist;

        //keeps enemy from wandering too far from startingPos
        randomPos += startingPoint;

        NavMeshHit hit;
        //prevents enemy from attempting to leave the navMesh 
        //when roaming, if edge is hit goes to that hit pos instead
        if (NavMesh.SamplePosition(randomPos, out hit, roamDist, 1))
        { agent.SetDestination(hit.position); }

        agent.stoppingDistance = stoppingDisOrig;
        isRoaming = false;
        someCo = null;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            animator.SetTrigger("Surprise");
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            //resets stopping dis for when player leaves the trigger
            agent.stoppingDistance = 0;
        }
    }
    
   
    IEnumerator beginAttack()
    {
       
        if (!isAttacking)
        {
            isDashing = true;
            //grouping dashes together, for creating more dynamic dashes
            for (int currDashNum = 0; currDashNum < numOfShield; currDashNum++)
            {
                // turns on orb sheild
                orbShield.gameObject.SetActive(true);
                //increases speed of ghost
                //agent.speed *= dashSpeedMutliplier;
                
               
                //determines time for dash
                yield return new WaitForSeconds(shieldTime);
                //returns to original speed
                //agent.speed /= dashSpeedMutliplier;

                //turns off orb sheild 
                orbShield.gameObject.SetActive(false);
                //waits for time between the next dash in loop
                yield return new WaitForSeconds(secBetweenShield);
                




            }
            //allows for orbAttack to begin
            isDashing = false;
        }
        //after dashes starts coroutine for Attack
        StartCoroutine(orbAttack());
    }



    IEnumerator orbAttack()
    {
        if (!isDashing)
        {
            
            isAttacking = true;//sets bool to true so dashing doesn't happen at the same time
            
            orbBullet.gameObject.SetActive(true); //turns on ghost bullet
            agent.speed *= dashSpeedMutliplier;

            animator.SetTrigger("Boo");

            yield return new WaitForSeconds(orbAttackTime);//keeps active for set time
            agent.speed /= dashSpeedMutliplier;

            orbBullet.gameObject.SetActive(false); //turns off ghost bullet

            yield return new WaitForSeconds(delayAfterAttack);
            
            isAttacking = false;//turns off bool so dash can begin
        }

    }

    public void takeDamage(int amount,Vector3 Dir)
    {
        //decreases health by damage amount
        HP -= amount;

        //stops the current coroutine so they are not conflicting
        if (someCo != null)
        {
            StopCoroutine(someCo);
            isRoaming = false;
        }
        agent.SetDestination(GameManager.instance.player.transform.position);

        StartCoroutine(flashColor());

        if (HP <= 0 && !isDying)
        {
            //set is dying so that it does not keep restarting death coroutine
            isDying = true;
            //stops walking to player
            agent.speed = 0;
            //stop all attacking/defense/roaming 
            StopAllCoroutines();
            //stops ghost from being a bloody drip during death animation
            model.material.color = colorOrig;
            //begins death animation
            StartCoroutine(Death());
           
        }
    }

    IEnumerator Death()
    {

        //updates game manager game goal and destroys the enemy
        GameManager.instance.updateGameGoal(-1);
        //death animation
        animator.SetTrigger("Death");
        //wait for animation to end
        yield return new WaitForSeconds(1.3f);
        modelSwitchPos = model.transform.position;
        modelSwitchRot = model.transform.rotation;
        Destroy(gameObject);
        if (isUsingNewModel)
        {
            Instantiate(modelSwitch, modelSwitchPos, modelSwitchRot);
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    private void SpawnEffect()
    {
        if(spawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(spawnEffectPrefab, transform.position + effectOffset, Quaternion.identity, transform);

            Destroy(effect, 2f);
        }
    }
}
