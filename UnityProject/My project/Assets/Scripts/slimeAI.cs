using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class slimeAI : MonoBehaviour, IDamage
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    [SerializeField] int HP;
    [SerializeField] int rotateSpeed;
    [SerializeField] int viewAngle;

    bool playerInRange;

    float angleToPlayer;

    Vector3 playerDir;

    Color colorOrig;

    // SlimeAI Code

    public enum SlimeAnimationState { Idle, Walk, Jump, Attack, Damage }

    public Face faces;
    public GameObject SmileBody;
    public SlimeAnimationState currentState;

    public Animator animator;
    public Transform[] waypoints;
    public int damType;

    private int m_CurrentWaypointIndex;

    private bool move;
    private Material faceMaterial;
    private Vector3 originPos;

    public enum WalkType { Patroll, ToOrigin, Chase }
    private WalkType walkType;

    // SlimeAI Code end

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        GameManager.instance.updateGameGoal(1);

        // SlimeAI Code

        originPos = transform.position;
        faceMaterial = SmileBody.GetComponent<Renderer>().materials[1];
        walkType = WalkType.Patroll;

        // SlimeAI Code end
    }

    // Update is called once per frame
    void Update()
    {
        // simple updates following along with lecture two. 

        if (playerInRange && canSeePlayer())
        {

        }
        else
        {
            currentState = SlimeAnimationState.Idle;
        }

        StartCoroutine(SwapFace());

    }

    void faceTarget()
    {
        Quaternion rotate = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rotate, Time.deltaTime * rotateSpeed);
    }

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - transform.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(transform.position, playerDir);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                    currentState = SlimeAnimationState.Attack;
                }
                else
                {
                    faceTarget();
                    currentState = SlimeAnimationState.Walk;
                    walkType = WalkType.Chase;
                }

                return true;

            }
        }
        return false;

    }
    //to derive from IDamage must be public void, must use the exact
    //syntax as IDamage
    public void takeDamage(int amount, Vector3 Dir)
    {
        HP -= amount;

        currentState = SlimeAnimationState.Damage;
        StartCoroutine(SwapFace());
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

    // SlimeAI Code

    public void WalkToNextDestination()
    {
        currentState = SlimeAnimationState.Walk;
        m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;
        agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);
        SetFace(faces.WalkFace);
    }
    public void CancelGoNextDestination() => CancelInvoke(nameof(WalkToNextDestination));

    void SetFace(Texture tex)
    {
        faceMaterial.SetTexture("_MainTex", tex);
    }

    IEnumerator SwapFace()
    {

        switch (currentState)
        {
            case SlimeAnimationState.Idle:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) yield return null;
                SetFace(faces.Idleface);
                break;

            case SlimeAnimationState.Walk:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Walk")) yield return null;

                agent.isStopped = false;
                agent.updateRotation = true;

                if (walkType == WalkType.ToOrigin)
                {
                    agent.SetDestination(originPos);
                    // Debug.Log("WalkToOrg");
                    SetFace(faces.WalkFace);
                    // agent reaches the destination
                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        walkType = WalkType.Patroll;

                        //facing to camera
                        transform.rotation = Quaternion.identity;

                        currentState = SlimeAnimationState.Idle;
                    }

                }
                else if (walkType == WalkType.Chase)
                {
                    SetFace(faces.WalkFace);

                }
                //Patroll
                else
                {
                    if (waypoints[0] == null || waypoints.Length == 0) yield return null;

                    agent.SetDestination(waypoints[m_CurrentWaypointIndex].position);

                    // agent reaches the destination
                    if (agent.remainingDistance < agent.stoppingDistance)
                    {
                        currentState = SlimeAnimationState.Idle;

                        //wait 2s before go to next destionation
                        Invoke(nameof(WalkToNextDestination), 2f);
                    }

                }
                // set Speed parameter synchronized with agent root motion moverment
                animator.SetFloat("Speed", agent.velocity.magnitude);


                break;

            case SlimeAnimationState.Jump:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump")) yield return null;

                SetFace(faces.jumpFace);
                animator.SetTrigger("Jump");

                //Debug.Log("Jumping");
                break;

            case SlimeAnimationState.Attack:

                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack")) yield return null;
                SetFace(faces.attackFace);
                animator.SetTrigger("Attack");

                // Debug.Log("Attacking");

                break;
            case SlimeAnimationState.Damage:

                // Do nothing when animtion is playing
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage0")
                     || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage1")
                     || animator.GetCurrentAnimatorStateInfo(0).IsName("Damage2")) yield return null;

                animator.SetTrigger("Damage");
                animator.SetInteger("DamageType", damType);
                SetFace(faces.damageFace);

                //Debug.Log("Take Damage");
                break;

        }
    }

    private void StopAgent()
    {
        agent.isStopped = true;
        animator.SetFloat("Speed", 0);
        agent.updateRotation = false;
    }
    // Animation Event
    public void AlertObservers(string message)
    {

        if (message.Equals("AnimationDamageEnded"))
        {
            // When Animation ended check distance between current position and first position 
            //if it > 1 AI will back to first position 

            float distanceOrg = Vector3.Distance(transform.position, originPos);
            if (distanceOrg > 1f)
            {
                walkType = WalkType.ToOrigin;
                currentState = SlimeAnimationState.Walk;
            }
            else currentState = SlimeAnimationState.Idle;

            //Debug.Log("DamageAnimationEnded");
        }

        if (message.Equals("AnimationAttackEnded"))
        {
            currentState = SlimeAnimationState.Idle;
        }

        if (message.Equals("AnimationJumpEnded"))
        {
            currentState = SlimeAnimationState.Idle;
        }
    }

    void OnAnimatorMove()
    {
        // apply root motion to AI
        Vector3 position = animator.rootPosition;
        position.y = agent.nextPosition.y;
        transform.position = position;
        agent.nextPosition = transform.position;
    }

}
