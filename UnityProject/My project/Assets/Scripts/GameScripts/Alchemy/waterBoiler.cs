using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterBoiler : MonoBehaviour
{
    private List<Item> itemsInBoiler = new List<Item>();
    private InteractionManager interactions;
    public bool inRange = false;

    private void Start()
    {
        interactions = FindObjectOfType<InteractionManager>();
    }

    public bool HasItem(Item item)
    {
        return itemsInBoiler.Contains(item);
    }

    public void AddItem(Item item)
    {
        itemsInBoiler.Add(item);
        Debug.Log(item.itemName + "added to Boiler");
    }

    public void ClearItems()
    {
        itemsInBoiler.Clear();
        Debug.Log("Boiler cleared");
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
