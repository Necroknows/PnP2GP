using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class NewEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Renderer model;
    [SerializeField] private Animator ani;
    [SerializeField] private Transform mouthPos;  // Position from where ranged attacks are fired
    [SerializeField] private Transform headPos;   // Position for the sightline raycast
    [SerializeField] private GameObject Claw;


    [SerializeField] private int HP;
    [SerializeField] private int maxHP;
    [SerializeField] private int rotateSpeed;
    [SerializeField] private int viewAngle;
    [SerializeField] private int roamDist;
    [SerializeField] private int roamPauseTime;

    [SerializeField] private GameObject projectile;  // Projectile for mouth attack
    [SerializeField] private float clawAttackRange;  // Range for claw attack
    [SerializeField] private float attackCooldown;   // Time delay between attacks
    [SerializeField] private float deathTime;
    [SerializeField] public float attackRange;

    private bool isAttacking = false;
    private bool playerInRange = false;
    private bool isRoaming = false;
    private bool isSniffing = false;

    private Vector3 playerDir;
    private Vector3 startingPos;
    private GameManager gameManager;

    //Spawn Effects
    public GameObject spawnEffectPrefab;
    public Vector3 effectOffset = new Vector3(0, 0, 0);
    public Image healthBarForeground;

    private void Start()
    {
        HP = maxHP;
        SpawnEffect();
        UpdateHealthBar();
        //Claw.SetActive(false);
        startingPos = transform.position;
        gameManager = GameManager.instance;
        gameManager.updateGameGoal(1); // Register with game goal
    }

    private void Update()
    {
        // Set animation speed based on movement
        ani.SetFloat("Speed", agent.velocity.normalized.magnitude);

        // Handle roaming, sniffing, and attack logic
        if (playerInRange && canSeePlayer())
        {
            // Face and attack player when in range and visible
            faceTarget();
            if (!isAttacking) ChooseAttack();
        }
        else
        {
            if (!isRoaming && !isSniffing)
            {
                StartCoroutine(RoamAndSniff());
            }
        }
    }

    private IEnumerator RoamAndSniff()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamPauseTime);

        // Randomly decide between roaming and sniffing
        isSniffing = Random.value > 0.5f;
        if (isSniffing)
        {
            ani.SetTrigger("SniffTrigger");
            yield return new WaitForSeconds(roamPauseTime);  // Wait while sniffing
            isSniffing = false;
        }



        // Get a random roam point
        Vector3 randomPos = Random.insideUnitSphere * roamDist + startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);


        isRoaming = false;
    }

    private void faceTarget()
    {
        Quaternion rotate = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        this.transform.LookAt(gameManager.player.transform);
    }

    private bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        float angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= viewAngle)
            {
                agent.SetDestination(GameManager.instance.player.transform.position);
                return true;
            }
        }
        return false;
    }

    private void ChooseAttack()
    {
        isAttacking = true;
        if (Vector3.Distance(transform.position, GameManager.instance.player.transform.position) <= clawAttackRange)
        {
            StartCoroutine(ClawAttack());
        }
        else
        {
            StartCoroutine(MouthAttack());
        }
    }

    private IEnumerator ClawAttack()
    {

        //Claw.SetActive(true);

        ani.SetTrigger("ClawAttackTrigger");
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
        //Claw.SetActive(false);
    }

    private IEnumerator MouthAttack()
    {
        ani.SetTrigger("MouthAttackTrigger");
        yield return new WaitForSeconds(0.5f);
        Instantiate(projectile, mouthPos.position, transform.rotation);
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    public void takeDamage(int amount, Vector3 Dir)
    {
        HP -= amount;
        UpdateHealthBar();
        StartCoroutine(flashRed());
        if (HP <= 0) StartCoroutine(death());
    }

    private void UpdateHealthBar()
    {
        float healthPercent = (float)HP / maxHP;
        healthBarForeground.fillAmount = healthPercent;
    }

    private IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = Color.white;  // Reset to default color
    }

    private IEnumerator death()
    {
        ani.SetTrigger("DeathTrigger");
        yield return new WaitForSeconds(deathTime);
        GameManager.instance.updateGameGoal(-1);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    private void SpawnEffect()
    {
        if (spawnEffectPrefab != null)
        {
            GameObject effect = Instantiate(spawnEffectPrefab, transform.position + effectOffset, Quaternion.identity, transform);

            Destroy(effect, 2f);
        }
    }
}
