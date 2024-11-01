using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ingredient", menuName = "Inventory/Ingredient")]


public class IngredientItem : InventoryItem
{
    //general
    public int ingredientID;  //ID of the ingredient

    private void Awake()
    {
        itemType = ItemType.Ingredient;   //set type automatically
    }

}//END