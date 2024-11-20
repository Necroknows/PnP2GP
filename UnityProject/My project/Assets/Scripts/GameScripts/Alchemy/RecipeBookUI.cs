//using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UI_Image = UnityEngine.UI.Image;

public class RecipeBookUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> recipeImages;
    [SerializeField] private Image recipeImageDisplay;


    public static RecipeBookUI instance;
    //private int recipeBookItemID = 600;

    public GameObject recipeBookPanel;
    public bool isShowing = false;

    public int currentPage;
    public List<AlchemyRecipe> recipes; //list of recipes from AlchemyManager

    //public UI_Image recipeImage;

    private void Awake()
    {
        Debug.Log("RecipeBookUI Awake()");

        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);        //ensures 1 copy
        }

        recipeBookPanel.SetActive(false);
        //recipes = AlchemyManager.instance.recipes;  //accesses recipes from AlchemyManager.cs
    }

    private void Start()
    {
        recipes = AlchemyManager.instance.recipes;
        Debug.Log("RecipeBookUI Start()");
    }

    private void OnEnable()
    {
        Debug.Log("RecipeBookUI OnEnable()");
    }

    private void Update()
    {
        //Debug.Log("RecipeBookUI Update() is running");

        //only allow page cycling if book is open
        if (recipeBookPanel.activeSelf)
        {
            if (Input.GetButtonDown("ArrowRight"))
            {
                Debug.Log("ArrowRight(right) pressed - calling NextPage()");
                NextPage();
            }
            else if (Input.GetButtonDown("ArrowLeft"))
            {
                Debug.Log("ArrowLeft(left) pressed - calling PrevPage()");
                PrevPage();
            }
        }
    }

    public void ShowRecipeBook()
    {
        recipeBookPanel.SetActive(true);
        currentPage = 0;
        ShowPage(currentPage);
        Debug.Log("Recipe Book opened - current page set to 0");
        isShowing = true;
    }

    public void HideRecipeBook()
    {
        recipeBookPanel.SetActive(false);
        isShowing= false;
    }

    public void ShowPage(int pageIndex)
    {
        Debug.Log($"Recipe book panel active: {recipeBookPanel.activeSelf}");
        Debug.Log($"Recipe image display active: {recipeImageDisplay.gameObject.activeSelf}");
        Debug.Log($"recipeImageDisplay assigned: {recipeImageDisplay != null}");
        Debug.Log($"ShowPage called w/ pageIndex: {pageIndex}");
        Debug.Log($"recipeImages.Count: {recipeImages.Count}");



        if (recipeImages == null || recipeImages.Count == 0)
        {
            Debug.LogWarning("No recipe images assigned");
        }
        else if (pageIndex < 0 || pageIndex >= recipeImages.Count)
        {
            Debug.LogWarning($"Invalid pageIndex: {pageIndex}. recipeImages.Count: {recipeImages.Count}");
        }
        else
        {
            //set display to show selected image
            recipeImageDisplay.sprite = recipeImages[pageIndex];

            AlchemyRecipe recipe = recipes[pageIndex];
        }
    }

    public void NextPage()
    {
        if (currentPage < recipes.Count)
        {
            currentPage++;
            ShowPage(currentPage);
        }
    }

    public void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            ShowPage(currentPage);
        }
    }
}
