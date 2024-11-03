using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<Item> Items = new List<Item>();

    public Transform ItemContent;       //where all items are filled
    public GameObject InventoryItem;

    private void Awake()
    {
        instance = this;
    }

    public void AddItem(Item item)
    {
        if(item == null)
        {
            Debug.LogError("Attempted to add null item to inventory");
            return;
        }

        Debug.Log("Adding itme " + item.itemName);
        Items.Add(item);
        ListItems();        //update UI after adding items
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
    }

    public void ListItems()
    {

        //check itemcontent and inventoryitem are set
        if(ItemContent == null)
        {
            Debug.LogError("ItemContent not set in InventoryManager");
            return;
        }

        if (InventoryItem == null)
        {
            Debug.LogError("InventoryItem not set in InventoryManager");
            return;
        }
        

        //clear all items in inventory UI
        foreach (Transform child in ItemContent)
        {
            Destroy(child.gameObject);
        }

        //create UI elements for each item in inventory
        foreach (var item in Items)
        {
            if(item == null || item.itemIcon == null)
            {
                Debug.LogError("Item or item icon is null for item " + (item != null ? item.itemName : "null"));
                continue;
            }

            GameObject obj = Instantiate(InventoryItem, ItemContent);
            var itemName = obj.transform.Find("itemName")?.GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("itemIcon")?.GetComponent<Image>();
            
            if(itemName == null)
            {
                Debug.LogError("Failed to find itemName in inventoryItem prefab");
                continue;
            }
            if(itemIcon == null)
            {
                Debug.LogError("Failed to find itemIcon in inventoryItem prefab");
                continue;
            }

            itemName.text = item.itemName;
            itemIcon.sprite = item.itemIcon;
            Debug.Log("Added item to UI " + item.itemName);
        }
    }
}//END