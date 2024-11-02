using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Item_script : MonoBehaviour
{
    public Item item;       //main obj to copy

    public Item heldItemProperties;     //copy of item w/ edited changes

    public TMP_Text itemNameText;

    public TMP_Text itemAmountText;

    public RawImage itemImage;

    public void SetItem(Item item)
    {
        //checks if modified
        if(heldItemProperties == null)
        {
            //if not, copy item
            SetCurrentHeldItemProperties(item);
        }

        //updates UI
        itemNameText.text = heldItemProperties.itemName;
        itemAmountText.text = heldItemProperties.currentAmount.ToString();
        itemImage.texture = heldItemProperties.itemIcon.texture;
    }

    public void SetCurrentHeldItemProperties(Item item)
    {
        //creates a copy of the item we put in

        heldItemProperties = ScriptableObject.CreateInstance<Item>();   //creates new Item scriptable object

        //copies values
        heldItemProperties.itemID = item.itemID;
        heldItemProperties.itemName = item.itemName;
        heldItemProperties.itemIcon = item.itemIcon;
        heldItemProperties.currentAmount = item.currentAmount;
        heldItemProperties.maxAmount = item.maxAmount;
        heldItemProperties.itemType = item.itemType;

    }


}//END
