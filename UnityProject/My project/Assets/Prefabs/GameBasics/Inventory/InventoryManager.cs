using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public List<Item> Items = new List<Item>();

    public Transform ItemContent;       //where all items are filled
    public GameObject InventoryItem;    //prefab to instantiate
    private List<GameObject> inventoryItemInstances = new List<GameObject>(); //list of instantiated inventory items
    public int currentSelectedItem = 0; //tracks currently selected item

    public int recipeBookItemID = 600;

    //Inventory Item Highlight
    private Vector3 defaultScale = new Vector3(1, 1, 1);            //default scale of inventory item
    private Vector3 highlightScale = new Vector3(1.2f, 1.2f, 1.2f); //scale of highlighted inventory item


    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            SelectNextItem();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            SelectPrevItem();
        }

        HighlightSelected();        //visually indicates selected item
        
        CheckForRecipeBook();       //if recipe book item is selected...

    }

    private void CheckForRecipeBook()
    {
        //get current selected item
        Item selectedItem = Items[currentSelectedItem];

        //check if recipe book item is selected in inventory
        if(selectedItem != null && selectedItem.itemID == recipeBookItemID)
        {
            //show recipe book if selected is the recipe book
            RecipeBookUI.instance.ShowRecipeBook();
        }
        else
        {
            //hide recipe book if any other item is selected
            RecipeBookUI.instance.HideRecipeBook();
        }
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

        Debug.Log("Adding itme " + item.itemName);
        Items.Add(item);
        ListItems();        //update UI after adding items
    }

    public void RemoveItem(Item item)
    {
        Items.Remove(item);
    }

    public void ListItems()
    {

        //clear all previous UI items
        foreach (Transform child in ItemContent)
        {
            Destroy(child.gameObject);
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

            itemName.text = item.itemName;

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
}//END