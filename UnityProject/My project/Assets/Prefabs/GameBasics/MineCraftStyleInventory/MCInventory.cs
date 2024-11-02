using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MCInventory : MonoBehaviour
{

    public static Inventory instance { get; set; }

    public GameObject[] inventoryItemArray;

    public InventoryManager inventoryManager;

    private void Awake()
    {
        instance = this;
    }

    public int GetItemAmount(Item item)
}
