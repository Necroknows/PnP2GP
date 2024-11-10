using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeStand : MonoBehaviour
{
    private AlchemyManager alchemy;
    [SerializeField] Mortar mortar;
    [SerializeField] waterBoiler boiler;
    public AlchemyRecipe currRecipe;
    

    void Start()
    {
        alchemy = FindObjectOfType<AlchemyManager>();

    }

    void Update()
    {
        
    }

    void SetRecipe(AlchemyRecipe recipe)
    {
        currRecipe = recipe;
    }

    public AlchemyRecipe GetCurrentRecipe => currRecipe;


}
