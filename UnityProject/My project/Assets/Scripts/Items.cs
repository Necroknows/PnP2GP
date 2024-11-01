using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public class InventoryItem : ScriptableObject
{
    public string itemName;
    public GameObject itemModel;

    public Sprite icon;
    public ItemType itemType;

    public enum ItemType
    {
        Herb,       //Unaltered, found in the wild
        Ingredient, //Herb that has been prepared
        Potion,     //Completed brew
        Weapon,     //Weapon
    }
}//END
