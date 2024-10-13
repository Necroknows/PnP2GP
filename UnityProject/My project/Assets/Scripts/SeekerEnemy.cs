using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SeekerEnemy : MonoBehaviour, IDamage
{

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer model;
    //where the seeker sees from
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    //seeker HP
    [SerializeField] int HP;
    //how fast the seeker rotates towards the pumpkin
    [SerializeField] int rotateSpeed;
    [SerializeField] int viewAngle;

    //gets pumpkin direction
    Vector3 pumpkinDirection;
    float angleToPumpkin;

    //if pumpkin is in range
    bool pumpkinInRange;

    Color colorOrigin;

    // Start is called before the first frame update
    void Start()
    {
        colorOrigin = model.material.color;
        GameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (pumpkinInRange && canSeePumpkin())
        {

        }
    }

    bool canSeePumpkin()
    {
        //define direction of pumpkin
        pumpkinDirection = GameManager.instance.pumpkin.transform.position - headPos.position;

        angleToPumpkin = Vector3.Angle(pumpkinDirection, transform.forward);
        Debug.DrawRay(headPos.position, pumpkinDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, pumpkinDirection, out hit))
        {
            if (hit.collider.CompareTag("Pumpkin") && angleToPumpkin < viewAngle)
            {
                //set destination to pumpkin using nav mesh agent
                agent.SetDestination(GameManager.instance.pumpkin.transform.position);

                //checking if pumkpin is inside stopping distance to make seeker face pumkpin
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    facePumpkin();
                }
            }

            //pumpkin can be seen
            return true;
        }

        return false;
    }

    void facePumpkin()
    {
        //rotates seeker toward pumpkin using quaternion
        //allows for smooth rotation, excludes pumpkin Y direction to avoid jittering
        Quaternion rot = Quaternion.LookRotation(new Vector3(pumpkinDirection.x, 0, pumpkinDirection.z));
        //using only transform.rotation = rot will cause instant rotation i.e. snapping
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * rotateSpeed);
    }

    //check if pumpkin is in range
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pumpkin"))
        {
            pumpkinInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pumpkin"))
        {
            pumpkinInRange = false;
        }
    }

    //changes color on damage
    IEnumerator flashColor()
    {
        //change color on damage
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrigin;
    }

    public void takeDamage(int damageAmount)
    {
        //take damage
        HP -= damageAmount;

        //set destination to pumpkin using nav mesh agent
        //agent.SetDestination(GameManager.instance.pumpkin.transform.position);
        //^removing because the seeker does not react to player on damage

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            //subtract from win condition
            GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

}
