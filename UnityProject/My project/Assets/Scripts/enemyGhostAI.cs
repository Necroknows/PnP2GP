using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyGhostAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int HP;
    [SerializeField] int rotateSpeed;
    [SerializeField] int viewAngle;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    bool isShooting;
    bool playerInRange;

    float angleToPlayer;

    Vector3 playerDir;

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
        // simple updates following along with lecture two. 

        playerDir = GameManager.instance.player.transform.position - transform.position;
        if (playerInRange)
        {
            if (canSeePlayer())
            {


                agent.SetDestination(GameManager.instance.player.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }
            }

        }

    }

    void faceTarget()
    {
        //sets the enemy to face the target/player
        Quaternion rotate = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rotate, Time.deltaTime * rotateSpeed);
    }

    bool canSeePlayer()
    {
        //gets the player location and angle
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPos.position, playerDir);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                //sets destination to player
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (!isShooting)
                {
                    StartCoroutine(shoot());


                    return true;
                }
            }
        }
        return false;

    }
    //to derive from IDamage must be public void, must use the exact
    //syntax as IDamage
    public void takeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = colorOrig;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
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

}
