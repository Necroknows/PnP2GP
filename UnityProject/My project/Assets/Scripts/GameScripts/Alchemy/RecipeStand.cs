using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RecipeStand : MonoBehaviour
{
    private AlchemyRecipe recipe = null;
    private InteractionManager interactions;
    public bool inRange = false;

    private void Start()
    {
        interactions = FindObjectOfType<InteractionManager>();
    }

    public bool IsRecipe(AlchemyRecipe _recipe)
    {
        return Equal.Equals(_recipe, recipe);
    }

    //add item to mortar
    public void SetRecipe(AlchemyRecipe _recipe)
    {
        recipe = _recipe;
        //Debug.Log("recipe" + recipe.recipeName + "added to Stand");
    }

    public AlchemyRecipe GetRecipe => recipe;

    public void ClearRecipe()
    {
        recipe = null;
        //Debug.Log("Stand cleared");
    }

    private void OnTriggerEnter(Collider other)
    {
        inRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;
    }

}//END
