using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item;
    private bool isPickedUp = false;        //check if item is picked up

void Pickup()
    {
        Debug.Log("Attempting item pick up" + item.itemName);

        if(InventoryManager.instance == null)
        {
            Debug.LogError("InventoryManager instance not found");
            return;     //exit if InventoryManager instance is not found
        }

        //only pick up item if it is not picked up already
        if (!isPickedUp)
        {
            isPickedUp = true;

            InventoryManager.instance.AddItem(item);
            
            Debug.Log("Item picked up" + item.itemName);

            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Item already picked up" + item.itemName);
        }
    }

    private void OnMouseDown()
    {
        Pickup();
    }
}
//END