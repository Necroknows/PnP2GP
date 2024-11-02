using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mortar : MonoBehaviour, IInteractive
{
    //assign a ground herb item to the mortar
    public InventoryItem groundHerb;

    public void Interact()
    {
        InventoryItem selectedHerb = Inventory.instance.GetItems().Find(item => item.itemType == InventoryItem.ItemType.Herb);
        if(selectedHerb != null)
        { Inventory.instance.RemoveItem(selectedHerb);
            Inventory.instance.AddItem(groundHerb);
            Debug.Log("Herb Ground");
        }
        else
        {
            Debug.Log("No herb in possession for grinding");
        }
    }
    
    public void GrindHerb(InventoryItem Herb)
    {
        if(Inventory.instance.HasItem(Herb))
        {
            Inventory.instance.RemoveItem(Herb);
            Inventory.instance.AddItem(groundHerb);
            Debug.Log($"{Herb.itemName} ground into {groundHerb.itemName}");
        }
        else
        {
            Debug.Log($"No {Herb.itemName} possessed for grinding");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
