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
            if (FindObjectWithRaycast())
            {
                //move to retrievable object if found
                agent.SetDestination(objectToRetrieve.transform.position);
                Debug.Log("Object Found" + objectToRetrieve.name);
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
            if (Vector3.Distance(transform.position, dropOffPoint.position) < 1f)
            {
                DropOffObject();
                Debug.Log("Object Dropped Off");
            }
        }
    }

    private bool FindObjectWithRaycast()
    {
        //cast ray forward from seeker to detect objects w/in range
        Vector3 rayOrigin = shootPos.position;              //use position for raycast
        Debug.DrawRay(rayOrigin, transform.forward, Color.red);

        RaycastHit hit;

        //cast ray forward to detect objects in range
        if (Physics.Raycast(rayOrigin, transform.forward, out hit, detectionRange, objectMask))
        {
            //check if hit object is retrievable & w/in certain range
            if (hit.collider.CompareTag("Retrievable"))
            {
                float distanceToObject = Vector3.Distance(transform.position, hit.collider.transform.position);
                if (distanceToObject < detectionRange)       //detection threshold
                {
                    //ensure there are no obstacles in the way
                    if (!Physics.Raycast(rayOrigin, hit.point, detectionRange, obstacleMask))
                    {
                        //check if hit object is retrievable
                        objectToRetrieve = hit.collider.gameObject;         //assign detected object to objectToRetrieve
                        Debug.Log("Hit Object Layer: " + objectToRetrieve.name);
                        return true;
                    }
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
        if (other.CompareTag("Retrievable"))
        {
            objectToRetrieve = other.gameObject;        //assigns detected object to objectToRetrieve
            Debug.Log("Object Entered: " + objectToRetrieve.name);
            PickUpObject();                             //automatically picks up object
        }
    }
    //called to pick up object
    void PickUpObject()
    {
        if (objectToRetrieve != null)
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
            objectToRetrieve = null;                                //clear reference to object
            hasObject = false;                                      //update carrying status
            Debug.Log("Object Dropped Off");
        }
    }
}
