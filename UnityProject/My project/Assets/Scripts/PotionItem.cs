using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potion")]

public class PotionItem : InventoryItem
{
    public int effect;  //which effect the potion has
    public int potionID;  //ID of the potion

    private void Awake()
    {
        itemType = ItemType.Potion;   //set type automatically
    }
}
