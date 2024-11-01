using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


public class InventoryItem : ScriptableObject
{
    //general
    public string itemName;

    //visual
    public GameObject itemModel;
    public Sprite icon;
    public ItemType itemType;

    //animation
    private Vector3 startPos;
    public float hoverHeight = 0.5f;
    public float hoverSpeed = 3.0f;
    public float hoverRange = 0.5f;

    //reference to player controller
    private PlayerController playerController;


    public enum ItemType
    {
        Herb,       //Unaltered, found in the wild
        Ingredient, //Herb that has been prepared
        Potion,     //Completed brew
        Weapon,     //Weapon
    }
}//END
