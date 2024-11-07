using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Item", menuName = "Item/Create")]

public class Item : ScriptableObject
{
    public int itemID;  //ID of the item
    public string itemName; //Name of the item
    public Sprite itemIcon; //Icon of the item
    public Sprite[] animatedIconFrames; //array of sprites for animated icon
    public float animationFrameRate = 2.5f; //frame rate of the animation

}//END