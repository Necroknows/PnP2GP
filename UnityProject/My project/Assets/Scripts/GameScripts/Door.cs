using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] Animator anim;
    
    [SerializeField] bool staysClosed;
    

    bool isOpen = true;
    // Start is called before the first frame update
    void Start()
    {
        if (!staysClosed)
        {
            anim.SetTrigger("Close");
            isOpen = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SwitchDoorState()
    {
        if(staysClosed && isOpen)
        {
            anim.SetTrigger("Close");
            isOpen = false;
        }
        if(!staysClosed && !isOpen)
        {
            anim.SetTrigger("Open");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null && !staysClosed)
        {
            SwitchDoorState();
        }
    }

    //On enter of the trigger calls for door to close
    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            SwitchDoorState();
        }
    }
}
