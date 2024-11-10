using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest Item", menuName = "Quest Item")]
public class QuestItem : ScriptableObject
{
    [SerializeField] public Item item;
    [SerializeField] int numToRetrieve;

    public int GetNumToRetrieve()
    {
        return numToRetrieve;
    }

    public string GetItemName()
    {
        return item.itemName;
    }
}
