using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyManager : MonoBehaviour, IInteractive
{
    //make it a singleton, bc there's only one
    public static AlchemyManager instance;

    //general
    public GameObject waterBoiler;
    public GameObject mortar;
    public Item waterItem;      //ref to water scriptable object
    public Item herbItem;       //ref to herb scriptable object

    //list of all recipes
    public List<AlchemyRecipe> recipes = new List<AlchemyRecipe>();

    private void Awake()
    {
        instance = this;
    }

    public void Update()
    {
        //only process if player is holding item & clicking LMB
        if(Input.GetKeyUp(KeyCode.E))
        {
            Item selectedItem = InventoryManager.instance.GetSelectedItem();

            if(selectedItem != null)
            {
                //check if interacting with boiler
                if(IsPointerOverObject(waterBoiler.gameObject))
                {
                    UseItemOnObject(selectedItem, waterBoiler, InventoryManager.instance);
                }
                //check if interacting with mortar
                else if (IsPointerOverObject(mortar.gameObject))
                {
                    UseItemOnObject(selectedItem, mortar, InventoryManager.instance);
                }
            }
        }

        //check if both are filled and create potion
        CraftPotion();

    }

    //attempt to craft based on a recipe
    public bool CraftItem(AlchemyRecipe recipe, InventoryManager inventory)
    {
        //check if player has all ingredients
        foreach (Item ingredient in recipe.ingredients)
        {
            if (!inventory.HasItem(ingredient))
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

    private bool IsPointerOverObject(GameObject targetObj)
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit) && hit.collider.gameObject == targetObj;
    }

    public void UseItemOnObject(Item item, GameObject targetObj, InventoryManager inventory)
    {
        //check if item is herb and target is mortar
        if (item.itemType == Item.ItemType.Herb && targetObj == mortar.gameObject)
        {
            //get the mortar component
            Mortar mortarComponent = targetObj.GetComponent<Mortar>();
            if (mortarComponent != null)
            {
                mortarComponent.AddItem(item);
                inventory.RemoveItem(item);
                Debug.Log(item.itemName + "added to Mortar");
                return;
            }
        }
        //check if water is item and boiler is target
        else if (item.itemType == Item.ItemType.Water && targetObj == waterBoiler.gameObject)
        {
            waterBoiler waterBoilerComponent = targetObj.GetComponent<waterBoiler>();
            if (waterBoilerComponent != null)
            {
                waterBoilerComponent.AddItem(item);
                inventory.RemoveItem(item);
                Debug.Log(item.itemName + "added to Boiler");
            }
        }
    }

    private void CraftPotion()
    {
        bool canCraft = false;

        //check if boiler & mortar have correct items
        if(waterBoiler.GetComponent<waterBoiler>().HasItem(waterItem) && mortar.GetComponent<Mortar>().HasItem(herbItem))
        {
            canCraft = true;
        }

        //craft possible potion
        if(canCraft)
        {
            AlchemyRecipe recipe = MatchRecipe();
            if(recipe != null)
            {
                //attempt to craft based on recipe
                InventoryManager.instance.AddItem(recipe.result);
                Debug.Log("Potion crafted & added to inventory");
                //clear objects after crafting
                waterBoiler.GetComponent<waterBoiler>().ClearItems();
                mortar.GetComponent<Mortar>().ClearItems();
            }
        }
    }

    private AlchemyRecipe MatchRecipe()
    {
        //place holder for matching recipes
        foreach (var recipe in recipes)
        {
            if(recipe.ingredients.Contains(waterItem) && recipe.ingredients.Contains(herbItem))
            {
                return recipe;
            }
        }
        return null;
    }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }
}//END