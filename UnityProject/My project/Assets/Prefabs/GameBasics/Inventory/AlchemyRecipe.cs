using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Alchemy Recipe", menuName = "Alchemy/Recipe")]

public class AlchemyRecipe : ScriptableObject
{
    public int recipeID;
    public string recipeName;
    public List<Item> ingredients;  //ingredients needed
    public Item result; //result of the recipe
    public Sprite recipeImage;
}
