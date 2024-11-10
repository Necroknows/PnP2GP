using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create")]

public class Item : ScriptableObject
{
    //general
    public int itemID;  //ID of the item
    public string itemName; //Name of the item
    
    //visual
    public Sprite itemIcon; //Icon of the item
    public Sprite[] animatedIconFrames; //array of sprites for animated icon
    public float animationFrameRate = 2.5f; //frame rate of the animation
    public int SpawnWithStack = 1;
    private int objectsInStack;

    public float hoverHeight = 0.5f;
    public float hoverSpeed = 3.0f;
    public float hoverRange = 0.5f;

    //interactive
    //public GameObject itemModel;
    public ItemType itemType;

    public static List<int> plantItemIDs = new List<int>();

    private void OnEnable()
    {
        if(!plantItemIDs.Contains(itemID))
        {
            objectsInStack = SpawnWithStack;
            plantItemIDs.Add(itemID);
        }
    }

    public int GetStack => objectsInStack;

    public void AddStack(int stack)
    {
        objectsInStack += stack;
    }

    public enum  ItemType
    {
        Herb,       //unaltered, found in the wild
        Ingredient, //herb that has been prepared
        Water,      //water
        Potion,     //completed recipe
        Weapon,     //weapon
        
    }
}//END