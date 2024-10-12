using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ZombieAI : MonoBehaviour, IDamage
{
    public Transform player;

    // serialized fields for tuning 
    [SerializeField] float moveSpeed; // enemy movement speed 
    [SerializeField] float attackRange;// how far can they reach with thier attack 
    [SerializeField] GameObject Bullet;// the type of thrown object they are releasing 
    [SerializeField] Transform ShootPOS;// the point from which the bullet spawns 
    [SerializeField] float AtkDelay;// how long in between attacks 
    [SerializeField] float BulletSpeed;// how fast is the bullet moving 
    [SerializeField] int HP;
    private bool isAttacking = false;// is the zombie currently attacking 



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.LookAt(player);
        }
        // gets the distance to the player object 
        float DistanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        // move towards the player if out of attack range

        if (DistanceToPlayer > attackRange && !isAttacking)
        {
            MoveTowardsPlayer();
        }
        else if(DistanceToPlayer <=attackRange && !isAttacking) 
        {
            StartCoroutine(ThrowPoisonBullet());
        }

    }
        void MoveTowardsPlayer()
        {
            // move toward the player maintaining distance 
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(player);
        }
        IEnumerator ThrowPoisonBullet()
        {
            isAttacking = true;

            yield return new WaitForSeconds(AtkDelay);

            GameObject poisonBullet = Instantiate(Bullet, ShootPOS.position, ShootPOS.rotation);
            Rigidbody rb= poisonBullet.GetComponent<Rigidbody>();
            rb.velocity = (player.position - transform.position).normalized*BulletSpeed;
            isAttacking=false;
        }

    public void takeDamage(int amount)
    {
        // reduce hp by damage amount 
       HP-=amount;
        // if health drops to zero destroy enemie object 
        if (HP <= 0)
        {
            GameManager.instance.updateGameGoal(-1);

            Destroy(gameObject);
        }
    }
}
