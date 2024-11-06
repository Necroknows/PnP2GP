using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyManager : MonoBehaviour
{
    //make it a singleton, bc there's only one
    public static AlchemyManager instance;

    //holds list of all recipes
    public List<AlchemyRecipe> recipes = new List<AlchemyRecipe>();

    private void Awake()
    {
        instance = this;
    }

    //attempt to craft based on a recipe
    public bool CraftItem(AlchemyRecipe recipe, InventoryManager inventory)
    {
        //check if player has all ingredients
        foreach (Item ingredient in recipe.ingredients)
        {
            if(!inventory.HasItem(ingredient))
            {
                Debug.Log("Missing ingredient " + ingredient.itemName);
                return false;
            }
        }

        //remove ingredients from inventory
        foreach (Item ingredient in recipe.ingredients)
        {
            inventory.RemoveItem(ingredient);
        }

        //add resulting item to inventory
        inventory.AddItem(recipe.result);
        Debug.Log("Crafted " + recipe.result.itemName);

        return true;
    }



}//END