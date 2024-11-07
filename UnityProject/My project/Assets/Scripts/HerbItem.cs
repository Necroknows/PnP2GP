using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Herb", menuName = "Inventory/Herb")]

public class HerbItem : Item
{
    public int herbID;  //ID of the herb

    private void Awake()
    {
        itemType = ItemType.Herb;   //set type automatically
    }

}//END