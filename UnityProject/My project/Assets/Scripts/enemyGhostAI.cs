using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyGhostAI : MonoBehaviour, IDamage
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] GameObject orb;
   // [SerializeField] Transform orbPos;
    [SerializeField] Transform headPos;
    //[SerializeField] Animator animator;

    [SerializeField] int HP;
    [SerializeField] int numOfDashes;
    [SerializeField] int secBetweenDashes;
    [SerializeField] int rotateSpeed;
    [SerializeField] int dashSpeedMutliplier;

    Vector3 playerDir;

    [SerializeField] GameObject bullet;
    [SerializeField] float dashTime;


    bool isOrbActive;
    bool isDashing;
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

            if (!isDashing)
            {
                //coroutine to shoot when player is in range
                StartCoroutine(dash());
                
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
            orbActiveToggle();
            //determines time for dash
            yield return new WaitForSeconds(dashTime);
            //returns to original speed
            agent.speed /= dashSpeedMutliplier;
            //turns off orb sheild 
            orbActiveToggle();
            //waits for time between the next dash in loop
            yield return new WaitForSeconds(secBetweenDashes);
        }
        isDashing = false;

    }
    public void orbActiveToggle()
    //turns the orb sheild on and off
    {
            if (!isOrbActive)
            {
                isOrbActive = true;
                orb.gameObject.SetActive(true);
            }
            else if (isOrbActive)
            {
                orb.gameObject.SetActive(false);
                isOrbActive = false;
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
