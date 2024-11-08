using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour
{
   // [SerializeField] GameObject door;
    [SerializeField] Animator anim;
    [SerializeField] enum DoorType { doorNormal, doorLockOffArea};
    [SerializeField] DoorType doorType;
    
    

    bool isOpen = true;
    // Start is called before the first frame update
    void Start()
    {
        
        if(doorType == DoorType.doorNormal)
        {
            //SwitchDoorState(false);
            isOpen = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    void SwitchDoorState(bool shouldOpen)
    {
        
        //switches the door being opened and closed
        if(shouldOpen == true)
        {
            isOpen = true;
            anim.SetTrigger("Open");
            
        }
        if (shouldOpen == false)
        {
            isOpen = false;
            anim.SetTrigger("Close");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(doorType == DoorType.doorNormal && !isOpen)
        {
            SwitchDoorState(true);
        }
    }

    //On enter of the trigger calls for door to close
    private void OnTriggerExit(Collider other)
    {
        //closes door after a player passes through
        if (other.GetComponent<PlayerController>() != null && isOpen)
        {
            SwitchDoorState(false);
        }
        //if (doorType == DoorType.doorNormal && !isOpen)
        //{
        //    SwitchDoorState(false);
        //}


    }
}
