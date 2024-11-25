using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterBoiler : MonoBehaviour
{
    private List<Item> itemsInBoiler = new List<Item>();
    private InteractionManager interactions;

    private void Start()
    {
        interactions = FindObjectOfType<InteractionManager>();
    }

    public List<Item> GetItems => itemsInBoiler;

    public bool HasItem(Item item)
    {
        return itemsInBoiler.Contains(item);
    }

    public void RemoveItem(Item item)
    {
        itemsInBoiler.Remove(item);
        //Debug.Log(item.itemName + "removed from Boiler");
    }

    public void AddItem(Item item)
    {
        itemsInBoiler.Add(item);
        //Debug.Log(item.itemName + "added to Boiler");
    }

    public void ClearItems()
    {
        itemsInBoiler.Clear();
        //Debug.Log("Boiler cleared");
    }

}//END
