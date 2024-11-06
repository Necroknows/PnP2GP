using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI_Image = UnityEngine.UI.Image;

public class RecipeBookUI : MonoBehaviour
{

    public static RecipeBookUI instance;
    private int recipeBookItemID = 600;

    public GameObject recipeBookPanel;
    
    private int currentPage = 0;
    private List<AlchemyRecipe> recipes; //list of recipes from AlchemyManager

    public UI_Image recipeImage;

    private void Awake()
    {
        instance = this;
        recipeBookPanel.SetActive(false);
        recipes = AlchemyManager.instance.recipes;  //accesses recipes from AlchemyManager.cs
    }

    private void Update()
    {
        //only allow page cycling if book is open
        if(recipeBookPanel.activeSelf)
        {
            if(Input.GetButtonDown("ArrowRight"))
            {
                NextPage();
            }
            else if (Input.GetButtonDown("ArrowLeft"))
            {
                PrevPage();
            }
        }
    }

    public void ShowRecipeBook()
    {
        recipeBookPanel.SetActive(true);
        currentPage = 0;
        ShowPage(currentPage);
        Debug.Log("Recipe Book Panel Activated");
    }
 

    public void HideRecipeBook()
    {
        recipeBookPanel.SetActive(false);
    }

    public void ShowPage(int pageIndex)
    {
        if(pageIndex >= 0 && pageIndex < recipes.Count)
        {
            AlchemyRecipe recipe = recipes[pageIndex];

            //display recipe page image
            if(recipe.recipeImage != null)
            {
                recipeImage.sprite = recipe.recipeImage;
            }
            else
            {
                recipeImage.sprite = null;
            }
        }
    }

    public void NextPage()
    {
        if(currentPage < recipes.Count -1)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PrevPage()
    {
        if(currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }
}
