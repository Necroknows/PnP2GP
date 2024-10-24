using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Seeker : MonoBehaviour
{
    //fields
    [SerializeField] Transform dropOffPoint;    //drop off point to for retrieved objects
    [SerializeField] float detectionRange;      //range for seeker to detect objects
    [SerializeField] float speed;               //speed of seeker
    [SerializeField] Transform shootPos;        //position to shoot raycast from
    [SerializeField] LayerMask objectMask;      //to detect retrievable objects
    [SerializeField] LayerMask obstacleMask;    //to detect obstacles
    [SerializeField] NavMeshAgent agent;        //navmesh agent for seeker movement
    [SerializeField] GameObject objectToRetrieve;   //current object to retrieve
    [SerializeField] float floorHieght;
    bool hasObject;                             //does the seeker have the object
    public Transform carryPosition;             //position on the seeker to carry the object


    // Start is called before the first frame update
    void Start()
    {
        //ensure navmesh is established
        if (agent == null)
        {
            agent.GetComponent<NavMeshAgent>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if seeker has no object, find one
        if (!hasObject)
        {
            //seeker to find object
            if (FindObjectWithOverlap())
            {
                //move to retrievable object if found
                agent.SetDestination(objectToRetrieve.transform.position);
                // Debug.Log("Object Found" + objectToRetrieve.name);
            }
            //if seeker has no object & doesn't see one, roam to find one
            else
            {
                //move randomly until retrievable object found
                Roam();
            }
        }
        //if seeker has an object, carry it to drop off point
        else
        {
            //carry retrievable object to drop off point
            agent.SetDestination(dropOffPoint.position);
            //Debug.Log("Taking Object To Drop Off" + objectToRetrieve.name);

            if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
            {
                DropOffObject();
                Debug.Log("Object Dropped Off");
            }
        }
    }

    private bool FindObjectWithOverlap()
    {
        if (hasObject) return false; //if seeker already has object, don't look for another

        //define a sphere around the seeker to detect objects
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRange, objectMask);

        foreach (var hitCollider in hitColliders)
        {
            //check if object is retrievable
            if (hitCollider.CompareTag("Retrievable")&& hitCollider.gameObject.transform.parent==null)
            {
                float distanceToObject = Vector3.Distance(transform.position, hitCollider.transform.position);

                //ensure object is w/in detection range & isn't an obstacle
                if (distanceToObject < detectionRange && !Physics.Raycast(transform.position, hitCollider.transform.position - transform.position, distanceToObject, obstacleMask))
                {
                   
                    
                        objectToRetrieve = hitCollider.gameObject; //assign detected object to objectToRetrieve
                    
                    Debug.Log("Object Found: " + objectToRetrieve.name);
                    return true;
                }
            }
        }

        //no object found
        return false;
    }

    private void Roam()
    {
        //implements roaming behavior
        if (agent.remainingDistance < 0.5f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * detectionRange;
            randomDirection += transform.position;
            NavMeshHit navHit;
            if (NavMesh.SamplePosition(randomDirection, out navHit, detectionRange, 1)) //samples position on navmesh
            {
                //move to new random position
                agent.SetDestination(navHit.position);
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Retrievable") && !hasObject)
        {
            objectToRetrieve = other.gameObject;  // assigns detected object to objectToRetrieve
            Debug.Log("Object Entered: " + objectToRetrieve.name);

            // Check if objectToRetrieve is properly assigned
            if (objectToRetrieve != null)
            {
                Debug.Log("Object Assigned Properly: " + objectToRetrieve.name);
                PickUpObject();  // automatically picks up object
            }
            else
            {
                Debug.Log("ObjectToRetrieve is NULL!");
            }
        }
    }
    //called to pick up object
    void PickUpObject()
    {
        if (objectToRetrieve != null && !hasObject && objectToRetrieve.transform.position.y<2.01)
        {
            objectToRetrieve.transform.SetParent(transform);        //parents object to seeker
            objectToRetrieve.transform.localPosition = carryPosition.localPosition; //sets carry position
            hasObject = true;                                       //updates carry status
            Debug.Log("Object Retrieved" + objectToRetrieve.name);
        }
    }

    //called to drop off object
    void DropOffObject()
    {
        if (objectToRetrieve != null)
        {
            objectToRetrieve.transform.SetParent(null);             //unparent object
            Destroy(objectToRetrieve);                              //destroy object
           /* objectToRetrieve = null;  */                              //clear reference to object
            hasObject = false;                                      //update carrying status

            //we'll either update game goals for the enemy in game manager or UI manager
            //Waiting on Jesse to confirm which script to use
            //gameManager.instance.UpdateGameGoal();
            Debug.Log("Object Dropped Off");
        }
    }
    
   

}
