using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;
    public static GameManager manager;
    public static InteractionManager interactables;

    public List<Item> Items = new List<Item>();

    [SerializeField] private RectTransform inventoryUI;
    public Transform ItemContent;       //where all items are filled
    public GameObject InventoryItem;    //prefab to instantiate
    [SerializeField] TextMeshProUGUI selectedItemName;
    private List<GameObject> inventoryItemInstances = new List<GameObject>(); //list of instantiated inventory items
    public int currentSelectedItem = 0; //tracks currently selected item

    public int recipeBookItemID = 600;

    //Inventory Item Highlight
    private Vector3 defaultScale = new Vector3(1, 1, 1);            //default scale of inventory item
    private Vector3 highlightScale = new Vector3(1.2f, 1.2f, 1.2f); //scale of highlighted inventory item


    private void Awake()
    {
        instance = this;
        manager = FindObjectOfType<GameManager>();
        interactables = FindObjectOfType<InteractionManager>();
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            SelectNextItem();
        }
        else if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            SelectPrevItem();
        }
        else if (Input.GetKeyUp(KeyCode.Q) && Items[currentSelectedItem].itemType == Item.ItemType.Potion)
        {
            UsePotion();
        }

        HighlightSelected();        //visually indicates selected item
        
        CheckForRecipeBook();       //if recipe book item is selected...

    }

    private void CheckForRecipeBook()
    {
        //check if recipebookUI.instance is available
        if(RecipeBookUI.instance == null)
        {
            RecipeBookUI.instance = FindObjectOfType<RecipeBookUI>();
            if(RecipeBookUI.instance == null)
            {
                Debug.LogWarning("RecipeBookUI.instance is STILL Null & may not have been initialized.");
                return;
            }
        }

        //ensure selectedItem is properly assigned
        if (Items.Count > 0 && currentSelectedItem >= 0 && currentSelectedItem < Items.Count)
        {
            //get current selected item
            Item selectedItem = Items[currentSelectedItem];

            //check if recipe book item is selected in inventory
            if (selectedItem != null && selectedItem.itemID == recipeBookItemID)
            {
                Debug.Log("Recipe Book selected - calling ShowRecipeBook");

                //show recipe book if selected is the recipe book
                RecipeBookUI.instance.ShowRecipeBook();
            }
            else
            {
                //Debug.Log("Other item selected - calling HideRecipeBook");

                //hide recipe book if any other item is selected
                RecipeBookUI.instance.HideRecipeBook();
            }
        }
        else
        {
            Debug.LogWarning("No Items in Inventory or Invalid Selection");
            RecipeBookUI.instance.HideRecipeBook();
        }
    }

    public Item GetSelectedItem()
    {
        if(Items.Count > 0 && currentSelectedItem >= 0 && currentSelectedItem < Items.Count)
        {
            return Items[currentSelectedItem];
        }
        return null;
    }

    public void SelectPrevItem()
    {
        if (Items.Count < 1) return;

        currentSelectedItem = (currentSelectedItem - 1 + Items.Count) % Items.Count;

    }

    public void SelectNextItem()
    {
        if (Items.Count < 1) return;

        currentSelectedItem = (currentSelectedItem + 1) % Items.Count;
    }

    public void HighlightSelected()
    {
        //loop thru item instances in UI and set scale
        for (int i = 0; i < inventoryItemInstances.Count; i++)
        {
            //if item is instanced
            if (inventoryItemInstances[i] != null)
            {
                //set scale of selected item index
                inventoryItemInstances[i].transform.localScale = (i == currentSelectedItem) ? highlightScale : defaultScale;
                selectedItemName.text = Items[currentSelectedItem].itemName;
            }
        }
    }

    public void AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Attempted to add null item to inventory");
            return;
        }
        if (Items.Count == 0)
        {
            Debug.Log("Adding item " + item.itemName + " and setting it as the currently held item.");
            Items.Add(item);
            currentSelectedItem = 0;
            inventoryUI.gameObject.SetActive(true);
        }
        else if (HasItem(item))
        {
            int index = 0;
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] == item)
                {
                    index = i;
                    break;
                }
            }
            Debug.Log("Adding item " + item.itemName + " to stack.");
            Items.Add(item);
            Debug.Log("Current stack: " + Items[index].objectsInStack);
        }
        else
        {
            Debug.Log("Adding item " + item.itemName);
            Items.Add(item);
        }
        ListItems();        //update UI after adding items
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
        if (currentSelectedItem != 0)
        {
            currentSelectedItem--;
        }
        if (Items.Count == 0)
        {
            selectedItemName.text = string.Empty;
            inventoryUI.gameObject.SetActive(false);
        }
        ListItems();        //update UI after adding items
    }

    public void ListItems()
    {
        StopAllCoroutines();
        //clear all previous UI items
        foreach (Transform child in ItemContent)
        {
            if (child != null)
            {
                Destroy(child.gameObject);
            }
        }
        inventoryItemInstances.Clear();

        //create UI elements for each item in inventory
        foreach (var item in Items)
        {
            if (item == null || item.itemIcon == null)
            {
                Debug.LogError("Item or Item Icon is null for Item" + (item != null ? item.itemName : "null"));
                continue;
            }

            GameObject obj = Instantiate(InventoryItem, ItemContent);
            inventoryItemInstances.Add(obj);    //track this instance for highlighting

            var itemName = obj.transform.Find("itemName")?.GetComponent<TextMeshProUGUI>();
            var itemIcon = obj.transform.Find("itemIcon")?.GetComponent<Image>();

            if (itemName == null)
            {
                Debug.LogError("Failed to find itemName in inventoryItem prefab");
                continue;
            }
            if (itemIcon == null)
            {
                Debug.LogError("Failed to find itemIcon in inventoryItem prefab");
                continue;
            }

            itemName.text = item.itemName + " x" + item.objectsInStack;
            selectedItemName.text = itemName.text;

            //check if the item has animated frames
            if(item.animatedIconFrames != null && item.animatedIconFrames.Length > 0)
            {
                //start coroutine to handle animation for this item
                StartCoroutine(AnimateIcon(item.animatedIconFrames, itemIcon));
            }
            else
            {
                itemIcon.sprite = item.itemIcon;
            }
            
            Debug.Log("Added item to UI " + item.itemName);
        }

        //check itemcontent and inventoryitem are set
        if (ItemContent == null)
        {
            Debug.LogError("ItemContent not set in InventoryManager");
            return;
        }

        if (InventoryItem == null)
        {
            Debug.LogError("InventoryItem not set in InventoryManager");
            return;
        }

        HighlightSelected();
    }

    public bool HasItem(Item item)
    {
        return Items.Contains(item);
    }

    private IEnumerator AnimateIcon(Sprite[] frames, Image itemIcon)
    {
        int currentFrame = 0;

        while (true)
        {
            itemIcon.sprite = frames[currentFrame];
            currentFrame = (currentFrame + 1) % frames.Length;
            yield return new WaitForSeconds(0.1f); //adjust speed as needed
        }
    }

    private void UsePotion()
    {
        if (Items[currentSelectedItem].itemName == "Health Potion")
        {
            manager.playerScript.setHPOrig(manager.playerScript.getHPOrig() + 1);
            RemoveItem(Items[currentSelectedItem]);
            ListItems();
            manager.playerScript.updatePlayerUI();
            interactables.Interact("Maximum health increased!\nPress E", KeyCode.E);
        }
        else if (Items[currentSelectedItem].itemName == "Dash Potion" && manager.playerScript.GetFuel() != manager.playerScript.GetFuelMax())
        {
            if (manager.playerScript.canDash == false)
            {
                manager.playerScript.canDash = true;
                interactables.Interact("Press left Control while jumping to use a midair dash.", KeyCode.LeftControl);
            }
            manager.playerScript.SetFuel(manager.playerScript.GetFuelMax());
            RemoveItem(Items[currentSelectedItem]);
            ListItems();
        }
        else if (Items[currentSelectedItem].itemName == "Mana Potion")
        {
            manager.playerScript.setAmmo(manager.playerScript.getAmmoMax());
            RemoveItem(Items[currentSelectedItem]);
            ListItems();
            interactables.Interact("Ammo Refilled!\nPress E", KeyCode.E);
        }
    }

}//END