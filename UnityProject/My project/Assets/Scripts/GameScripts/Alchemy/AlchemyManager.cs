using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlchemyManager : MonoBehaviour
{
    //make it a singleton, bc there's only one
    public static AlchemyManager instance;
    private InteractionManager interactables;

    //general
    public waterBoiler waterBoiler;
    public Mortar mortar;
    public RecipeStand stand;
    public Item waterItem;      //ref to water scriptable object
    public Item herbItem;       //ref to herb scriptable object

    private bool playerInRange = false;

    //list of all recipes
    public List<AlchemyRecipe> recipes = new List<AlchemyRecipe>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        interactables = FindObjectOfType<InteractionManager>();
    }

    public void Update()
    {
        if (playerInRange) { 
        //only process if player is holding item & clicking Q
            Item selectedItem = InventoryManager.instance.GetSelectedItem();

            if(selectedItem != null)
            {
                //check if interacting with boiler
                if(IsPointerOverObject(waterBoiler.gameObject))
                {
                    interactables.Interact("Press Q to place Liquid into the Boiler", KeyCode.Q);
                    if (Input.GetKeyUp(KeyCode.Q))
                    {
                        interactables.StopInteract();
                        UseItemOnObject(selectedItem, waterBoiler.gameObject, InventoryManager.instance);
                    }
                }
                //check if interacting with mortar
                else if (IsPointerOverObject(mortar.gameObject))
                {
                    interactables.Interact("Press Q to place Material into the Mortar", KeyCode.Q);
                    if (Input.GetKeyUp(KeyCode.Q))
                    {
                        interactables.StopInteract();
                        UseItemOnObject(selectedItem, mortar.gameObject, InventoryManager.instance);
                    }
                }
                else if (IsPointerOverObject(stand.gameObject))
                {
                    interactables.Interact("Press Q to place Recipe into the Recipe Stand or E to Craft the Recipe", KeyCode.Q);
                    if (Input.GetKeyUp(KeyCode.Q))
                    {
                        interactables.StopInteract();
                        if (selectedItem != null && selectedItem.itemID == InventoryManager.instance.recipeBookItemID)
                        {
                            stand.SetRecipe(RecipeBookUI.instance.recipes[RecipeBookUI.instance.currentPage]);
                        }
                    }
                    else if (Input.GetKeyUp(KeyCode.E))
                    {
                        //check if both are filled and create potion
                        CraftPotion();
                    }
                }
                else
                {
                    if (interactables.animator.GetBool("IsOpen"))
                    {
                        interactables.StopInteract();
                    }
                }
            }
        }
    }

    //attempt to craft based on a recipe
    public bool CraftItem(AlchemyRecipe recipe)
    {
        //check if player has all ingredients
        foreach (Item ingredient in recipe.ingredients)
        {
            if (!mortar.HasItem(ingredient) && ingredient.itemType == Item.ItemType.Herb)
            {
                Debug.Log("Missing ingredient " + ingredient.itemName);
                return false;
            }
            else if (!waterBoiler.HasItem(ingredient) && ingredient.itemType == Item.ItemType.Water)
            {
                Debug.Log("Missing ingredient " + ingredient.itemName);
                return false;
            }
        }

        //add resulting item to inventory
        InventoryManager.instance.AddItem(recipe.result);
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
        bool canCraft = true;

        //check if boiler & mortar have correct items
        List<Item> list = new List<Item>();
        foreach (Item items in mortar.GetItems)
        {
            list.Add(items);
        }
        foreach (Item items in waterBoiler.GetItems)
        {
            list.Add(items);
        }
        List<Item> checks = new List<Item>();
        foreach (Item item in list)
        {
            checks.Add(item);
        }
        foreach (Item ingredient in stand.GetRecipe.ingredients)
        {
            if (!checks.Contains(ingredient))
            {
                canCraft = false;
                Debug.Log("Invalid items for recipe");
                break;
            }
            checks.Remove(ingredient);
        }

        //craft possible potion
        if (canCraft)
        {
            if (stand.GetRecipe != null)
            {
                //attempt to craft based on recipe
                InventoryManager.instance.AddItem(stand.GetRecipe.result);
                Debug.Log("Potion crafted & added to inventory.");
                //clear objects after crafting
                foreach (Item item in stand.GetRecipe.ingredients)
                {
                    if (item != null && item.itemType == Item.ItemType.Water)
                    {
                        waterBoiler.RemoveItem(item);
                        Debug.Log(item.itemName + "removed from waterBoiler.");
                    }
                    else if (item != null && item.itemType == Item.ItemType.Herb)
                    {
                        mortar.RemoveItem(item);
                        Debug.Log(item.itemName + "removed from mortar.");
                    }
                }
                if (waterBoiler.GetItems != null)
                {
                    for (int i = 0; i < waterBoiler.GetItems.Count; i++)
                    {
                        Item temp = waterBoiler.GetItems[i];
                        InventoryManager.instance.AddItem(waterBoiler.GetItems[i]);
                        Debug.Log(temp.itemName + "returned to player inventory.");
                        waterBoiler.RemoveItem(waterBoiler.GetItems[i]);
                        Debug.Log(temp.itemName + "removed from waterBoiler.");
                        i--;
                    }
                }
                if (mortar.GetItems != null)
                {
                    for (int i = 0; i < mortar.GetItems.Count; i++)
                    {
                        Item temp = mortar.GetItems[i];
                        InventoryManager.instance.AddItem(mortar.GetItems[i]);
                        Debug.Log(temp.itemName + "returned to player inventory.");
                        mortar.RemoveItem(mortar.GetItems[i]);
                        Debug.Log(temp.itemName + "removed from mortar.");
                        i--;
                    }
                }
            }
        }
        else
        {
            foreach (Item item in list)
            {
                InventoryManager.instance.AddItem(item);
                Debug.Log(item.itemName + " returned to player");
            }
            waterBoiler.GetComponent<waterBoiler>().ClearItems();
            mortar.GetComponent<Mortar>().ClearItems();
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

    private void OnTriggerEnter(Collider other)
    {
        playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        playerInRange = false;
    }
}//END