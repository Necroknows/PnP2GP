using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    private List<Item> itemsInMortar = new List<Item>();
    private InteractionManager interactions;

    private void Start()
    {
        interactions = FindObjectOfType<InteractionManager>();
    }

    public List<Item> GetItems => itemsInMortar;

    public bool HasItem(Item item)
    {
        return itemsInMortar.Contains(item);
    }

    public void RemoveItem(Item item)
    {
        itemsInMortar.Remove(item);
        //Debug.Log(item.itemName + "removed from Mortar");
    }

    //add item to mortar
    public void AddItem(Item item)
    {
        itemsInMortar.Add(item);
        //Debug.Log(item.itemName + "added to Mortar");
    }

    public void ClearItems()
    {
        itemsInMortar.Clear();
        //Debug.Log("Mortar cleared");

    }

}//END
