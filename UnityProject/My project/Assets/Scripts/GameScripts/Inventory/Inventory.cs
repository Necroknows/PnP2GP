using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    [SerializeField] private List<Item> items = new List<Item>();

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        Debug.Log($"{item.itemName} added to inventory");
    }

    public List<Item> GetItems()
    {
        return items;
    }

    public List<T> GetItemsOfType<T>() where T : Item
    {
        List<T> itemsOfType = new List<T>();
        foreach (Item item in items)
        {
            if (item is T specificItem)
            {
                itemsOfType.Add(specificItem);
            }
        }
        return itemsOfType;
    }

    public void RemoveItem(Item item)
    {
        if (items.Contains(item))
        {
            items.Remove(item);
            Debug.Log($"{item.itemName} removed from inventory");
        }
        else
        {
            Debug.Log($"{item.itemName} not found in inventory");
        }
    }

    public bool HasItem(Item item)
    {
        return items.Contains(item);
    }

    public void ClearInventory()
    {
        if(items.Count > 0)
        {
            items.Clear();
        }
    }


}//END
