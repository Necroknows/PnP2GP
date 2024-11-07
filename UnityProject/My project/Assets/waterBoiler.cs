using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyObject : MonoBehaviour
{

    public bool HasWater { get; set; }
    public bool HasPlant { get; set; }
    public void AddItem(Item item)
    {
        if (item.itemID == 600)
        {
            HasWater = true;
            Debug.Log("Water boiling");
        }
        else if (item.itemID == 100)
        {
            HasPlant = true;
            Debug.Log("Plant added");
        }
    }
}
