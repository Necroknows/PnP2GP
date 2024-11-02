using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCCell : MonoBehaviour
{

    public int CellIndex;

    public InventoryManager inventoryManager;

    public GameObject basicItemPrefab;

    public GameObject currentHeldItem;

    public void CreateItemInCell(Item item)
    {
        //delete child objects
        if (transform.childCount > 0)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        currentHeldItem = Instantiate(basicItemPrefab, transform.position, transform.rotation, transform);
        currentHeldItem.GetComponent<Item_script>().SetItem(item);    //update item values
    }

    public void OnClick()
    {
        bool haveAddedItem = false; //checks if cell is filled

        if (currentHeldItem != null && inventoryManager.itemInHand != null)
        {
            Item inventoryManagerItem = inventoryManager.itemInHand.GetComponent<Item_script>().heldItemProperties;
            Item cellItem = currentHeldItem.GetComponent<Item_script>().heldItemProperties;
            
            //checks for space to fit item in cell
            if (cellItem.currentAmount < cellItem.maxAmount && cellItem.itemName == inventoryManagerItem.itemName)
            {
                int diff = cellItem.maxAmount - cellItem.currentAmount;     //calculates the differents to fit proper amount
                if (diff >= inventoryManagerItem.currentAmount)              //there's enough space
                {
                    cellItem.currentAmount += inventoryManagerItem.currentAmount;
                    Destroy(inventoryManager.itemInHand);
                    inventoryManager.itemInHand = null;
                }
                else
                {
                    cellItem.currentAmount += diff;         //adds as much as possible
                    inventoryManagerItem.currentAmount -= diff;
                }

                if (inventoryManager.itemInHand != null)
                {
                    currentHeldItem.GetComponent<Item_script>().SetItem(null);        //updates values of item in cell visually
                    haveAddedItem = true;
                }

            }

            //unable to stack
            if (!haveAddedItem)
            {
                GameObject provCellItemHolder = currentHeldItem;        //saves currently held item to a new variable
                if (inventoryManager.itemInHand != null)                //checks if there's an item in hand
                {
                    currentHeldItem = inventoryManager.itemInHand.gameObject;   //switches items
                }
                else
                {
                    currentHeldItem = null;         //no item in hand means can't be assigned to cell
                }
                inventoryManager.itemInHand = provCellItemHolder;       //sets item in hand to the one that was in the cell
            }

            if (currentHeldItem != null)             //avoids modifying a gameobject that isn't there
            {
                currentHeldItem.transform.parent = transform;        //sets item to cell
                currentHeldItem.transform.position = transform.position;  //sets item to cell position
                currentHeldItem.transform.localScale = new Vector3(.8f, .8f, .8f);   //modifies scale of item

                Inventory.instance.inventoryItemArray[CellIndex] = currentHeldItem; //updates main inventory array w/ cell index

            }
            else
            {
                Inventory.instance.inventoryItemArray[CellIndex] = null; //updates main inventory array w/ cell index   
            }
        }
    }
}//END