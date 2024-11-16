using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SavedPlayerState : MonoBehaviour
{
    public static SavedPlayerState instance;
    // Start is called before the first frame update
    List<Item> savedItems = new List<Item>();

    float meterfill;

    int savedHP;

    private void Awake()
    {
        instance = this;
        savedHP = GameManager.instance.playerScript.getHPOrig();
    }


    public void SaveCurrState()
    {
        //holds them in the instance list for loading later
        for (int eachItem = 0; eachItem < InventoryManager.instance.Items.Count; eachItem++)
        {
            if (InventoryManager.instance.Items[eachItem] != null)
            { 
                savedItems.Add(InventoryManager.instance.Items[eachItem]); 
            }
        }
        //saves the players HP
        savedHP = GameManager.instance.playerScript.getHP();
        meterfill = DeathSpawnManager.instance.GetCurrentFillAmount();
    }

    public void DeathLoadState()
    {
        InventoryManager.instance.ClearInventory();
        
        //resets inventory to last checkpoint (or save) except ingredients items
        for (int eachItem = 0; eachItem < savedItems.Count; eachItem++)
        {
            if (savedItems[eachItem].itemType == Item.ItemType.Potion || savedItems[eachItem].itemType == Item.ItemType.Weapon)
            {
                InventoryManager.instance.AddItem(savedItems[eachItem]);
            }
            
        }
        InventoryManager.instance.ListItems();
        //sets the savedHP to the HP orig
        savedHP = GameManager.instance.playerScript.getHPOrig();
        GameManager.instance.playerScript.setHP(savedHP); //sets the players back to the orig
        GameManager.instance.playerSpawnPOS.transform.position = GameManager.instance.playerStartPOS.transform.position; //sets the spawn position to start

        DeathSpawnManager.instance.ResetMeter(); //resets the meter and despawns death

    }
    public void LoadLastSavedState()
    {
        InventoryManager.instance.ClearInventory();
        //resets inventory to last checkpoint (or save) except ingredients items
        for (int eachItem = 0; eachItem < savedItems.Count; eachItem++)
        {
            InventoryManager.instance.AddItem(savedItems[eachItem]);
        }
        InventoryManager.instance.ListItems();
        GameManager.instance.playerScript.setHP(savedHP);
        DeathSpawnManager.instance.SetFillAmount(meterfill);
    }

}
