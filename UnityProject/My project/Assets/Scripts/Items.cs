using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]

public class InventoryItem : ScriptableObject
{
    public string itemName;
    public GameObject itemModel;

    public Sprite icon;
    public ItemType itemType;

    public enum ItemType
    {
        Herb,       //Unaltered, found in the wild
        Potion,     //Completed brew
        Alcohol,    //Base ingredient
        Ingredient, //Herb/item that has been prepared
    }

}//END
