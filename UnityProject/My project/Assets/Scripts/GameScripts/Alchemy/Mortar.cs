using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour
{
    private List<Item> itemsInMortar = new List<Item>();
    private InteractionManager interactions;
    public bool inRange = false;

    private void Start()
    {
        interactions = FindObjectOfType<InteractionManager>();
    }

    public bool HasItem(Item item)
    {
        return itemsInMortar.Contains(item);
    }

    //add item to mortar
    public void AddItem(Item item)
    {
        itemsInMortar.Add(item);
        Debug.Log(item.itemName + "added to Mortar");
    }

    public void ClearItems()
    {
        itemsInMortar.Clear();
        Debug.Log("Mortar cleared");

    }

    private void OnTriggerEnter(Collider other)
    {
        inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;
    }

}//END
