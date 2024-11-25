using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    private InteractionManager interactables;
    private bool isPickedUp = false;        //check if item is picked up
    private bool isPlayerInRange = false;   //check if player is in range of item

    private void Start()
    {
        interactables = FindObjectOfType<InteractionManager>();
    }

    private void Update()
    {
        if(isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isPickedUp)
        {
            Pickup();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            //Debug.Log("Player in range of item" + item.itemName);
            interactables.Interact("Press E to pick up " + item.itemName, KeyCode.E);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            //Debug.Log("Player out of range of item" + item.itemName);
            interactables.StopInteract();
        }
    }

    public void Pickup()
    {
        //Debug.Log("Attempting item pick up" + item.itemName);

        if(InventoryManager.instance == null)
        {
            //Debug.LogError("InventoryManager instance not found");
            return;     //exit if InventoryManager instance is not found
        }

        //only pick up item if it is not picked up already
        if (!isPickedUp)
        {
            isPickedUp = true;

            InventoryManager.instance.AddItem(item);
           

            //Debug.Log("Item picked up" + item.itemName);

            Destroy(gameObject);
        }
        else
        {
            //Debug.Log("Item already picked up" + item.itemName);
        }
    }
}
//END