using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] Animator anim;
    [SerializeField] enum DoorType { doorNormal, doorLockOffArea, doorGoalWinOpens};
    [SerializeField] DoorType doorType;
    
    

    bool isOpen = true;
    // Start is called before the first frame update
    void Start()
    {
        if (doorType == DoorType.doorGoalWinOpens)
        {
            SwitchDoorState(false);
            isOpen = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //checks if the JOL mini game has been completed, calls the doors to open, check if it's already open
        if(GameManager.instance.GetGoalState() == true && doorType == DoorType.doorGoalWinOpens && !isOpen)
        {
            SwitchDoorState(true);
        }

    }

    void SwitchDoorState(bool open)
    {
        
        //switches the door being opened and closed
        if(open == true)
        {
            isOpen = false;
            anim.SetTrigger("Open");
            
        }
        if (open == false)
        {
            isOpen = true;
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
        if (doorType == DoorType.doorNormal && !isOpen)
        {
            SwitchDoorState(false);
        }


    }
}
