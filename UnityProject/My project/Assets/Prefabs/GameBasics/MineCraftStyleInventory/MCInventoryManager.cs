using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.ReorderableList;
using UnityEngine;

public class MCInventoryManager : MonoBehaviour
{
    public Transform cellParent;

    public GameObject itemInHand;

    public Vector3 offset;

    bool inventoryOpen = false;

    public GameObject InventoryMenu;

    public Item itemToRemove;

    public int amountToRemove;

    public GameObject itemToAdd;

    private void Start()
    {
        SetCells();
    }

    private void Update()
    {
        MoveItemInHand();

        OpenClose();

    }

    void MoveItemInHand()
    {
        if (itemInHand == null)
        { return; }

        //sets object in hand to desired position
        itemInHand.transform.parent = transform;
        itemInHand.transform.localScale = new Vector3(.5f, .5f, .5f);   //modifies scale when moving item
        Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f) + offset; //sets position of item in hand, required to click buttons
        itemInHand.transform.position = pos;        //sets item to actual position
    }

    public void SetCells()
    {
        int index = 0;

        //loop through all children of cellParent
        foreach (Transform cell in cellParent)
        {
            //prevents multiple different items in cell
            if (cell.childCount > 0)
            {
                foreach (Transform child in cell)
                {
                    Destroy(child.gameObject);
                }
            }

            //checks for an item in current inventory slot
            if (Inventory.instance.inventoryItemArray[index] != null)
            {
                //checks if item has modified value
                if (Inventory.instance.inventoryItemArray[index].GetComponent<MCItem_script>().heldItemProperties != null)
                {
                    cell.GetComponent<MCCell>().CreateItemInCell(Inventory.instance.inventoryItemArray[index].GetComponent<MCItem_script>().heldItemProperties.heldItemProperties);
                }
                else
                {
                    cell.GetComponent<MCCell>().CreateItemInCell(Inventory.instance.inventoryItemArray[index].GetComponent<MCItem_script>().item);
                }
                Inventory.instance.inventoryItemArray[index] = cell.GetComponent<MCCell>().currentHeldItem;
            }
            cell.GetComponent<MCCell>().CellIndex = index;
        }
    }

    void OpenClose()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryOpen = !inventoryOpen;
            InventoryMenu.SetActive(inventoryOpen);
        }
    }
}
//END