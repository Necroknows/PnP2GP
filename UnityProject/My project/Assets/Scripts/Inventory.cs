using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void AddItem(InventoryItem item)
    {
        items.Add(item);
        Debug.Log($"{item.itemName} added to inventory");
    }

    public List<InventoryItem> GetItems()
    {
        return items;
    }

    public List<T> GetItemsOfType<T>() where T : InventoryItem
    {
        List<T> itemsOfType = new List<T>();
        foreach (InventoryItem item in items)
        {
            if(item is T specificItem)
            {
                itemsOfType.Add(specificItem);
            }
        }
        return itemsOfType;
    }

    public void RemoveItem(InventoryItem item)
    {
        if(items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"{item.itemName} removed from inventory");
        }
        else
        {
            Debug.Log($"{item.itemName} not found in inventory");
        }
    }

    public bool HasItem(InventoryItem item)
    {
        return items.Contains(item);
    }


}//END
