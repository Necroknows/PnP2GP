using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] Animator animator;
    [SerializeField] Transform headPOS;

    [SerializeField] int roamDistance;
    [SerializeField] float detectDistance;
    [SerializeField] int roamPauseTime;

    [SerializeField] int HP;
    [SerializeField] int rotateSpeed;
    [SerializeField] int fieldOfView;

    bool isShooting;
    bool playerInRange;
    bool isRoaming;

    float angleToPlayer;
    public float stoppingDistanceOriginal;

    Vector3 playerDirection;
    Vector3 startingPOS;

    Color colorOriginal;

    Coroutine someCo;

    public Face faces;
    private Material faceMaterial;
    private Texture faceOriginal;
    public GameObject SmileBody;


    // Start is called before the first frame update
    void Start()
    {
        colorOriginal = model.material.color;
        GameManager.instance.updateGameGoal(1);
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        startingPOS = transform.position;
        stoppingDistanceOriginal = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        if (animator.GetFloat("Speed") <= 0.05)
            SetFace(faces.Idleface);
        animator.SetFloat("Speed", agent.velocity.normalized.magnitude);
        if (!CanSeePlayer())
        {
            if (!isRoaming && agent.remainingDistance < 0.01f)
            {
                agent.stoppingDistance = 0;
                someCo = StartCoroutine(Roam());
            }
        }
    }

    IEnumerator Roam()
    {
        isRoaming = true;
        SetFace(faces.Idleface);
        yield return new WaitForSeconds(roamPauseTime);
        SetFace(faces.WalkFace);

        agent.stoppingDistance = 0;
        Vector3 randomDistance = Random.insideUnitSphere * roamDistance;
        randomDistance += startingPOS;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDistance, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
        someCo = null;
    }

    bool CanSeePlayer()
    {
        playerDirection = GameManager.instance.player.transform.position - headPOS.position;
        angleToPlayer = Vector3.Angle(new Vector3(playerDirection.x, 0, playerDirection.z), transform.forward);
        Debug.DrawRay(headPOS.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPOS.position, playerDirection, out hit, detectDistance))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= fieldOfView)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    FaceTarget();
                    if (!isShooting)
                    {
                        StartCoroutine(Shoot());
                    }
                }


                agent.stoppingDistance = stoppingDistanceOriginal;

                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;
    }

    void FaceTarget()
    {
        Quaternion rotate = Quaternion.LookRotation(new Vector3(playerDirection.x, 0, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rotate, Time.deltaTime * rotateSpeed);
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
            agent.stoppingDistance = 0;
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        SetFace(faces.attackFace);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.6f);
        if (playerInRange)
        {
            playerInRange = true;
        }

        isShooting = false;
    }
    public void takeDamage(int amount, Vector3 Dir)
    {
        HP -= amount;

        if (someCo != null)
        {
            StopCoroutine(someCo);
            isRoaming = false;
        }

        agent.SetDestination(GameManager.instance.player.transform.position);
        
        StartCoroutine(FlashColor());
        StartCoroutine(Damaged());

        if (HP <= 0)
        {
            StartCoroutine(Dead());
        }
    }

    IEnumerator Damaged()
    {
        faceOriginal = faceMaterial.mainTexture;
        SetFace(faces.damageFace);
        animator.SetTrigger("Damage");
        animator.SetInteger("DamageType", 0);
        yield return new WaitForSeconds(0.5f);
        SetFace(faceOriginal);
    }

    IEnumerator Dead()
    {
        agent.isStopped = true;
        SetFace(faces.damageFace);
        animator.SetTrigger("Damage");
        animator.SetInteger("DamageType", 1);
        yield return new WaitForSeconds(1.5f);
        GameManager.instance.updateGameGoal(-1);
        Destroy(gameObject);
    }

    IEnumerator FlashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    public void AlertObservers(string message)
    {

        if (message.Equals("AnimationDamageEnded"))
        {
            // When Animation ended check distance between current position and first position 
            //if it > 1 AI will back to first position 

            float distanceOrg = Vector3.Distance(transform.position, startingPOS);
            if (distanceOrg > 1f)
            {

            }

            //Debug.Log("DamageAnimationEnded");
        }

        if (message.Equals("AnimationAttackEnded"))
        {

        }

        if (message.Equals("AnimationJumpEnded"))
        {

        }
    }


}