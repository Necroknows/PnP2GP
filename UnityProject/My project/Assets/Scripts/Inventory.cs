using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private List<InventoryItem> items = new List<InventoryItem>();

    public static Inventory instance;
    private void Awake()
    {
        if (instance != null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(InventoryItem item)
    {
        items.Add(item);
        Debug.Log($"{item.itemName} added to inventory");
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

    public List<InventoryItem> GetItems()
    {
        return new List<InventoryItem>(items);
    }

}//END
