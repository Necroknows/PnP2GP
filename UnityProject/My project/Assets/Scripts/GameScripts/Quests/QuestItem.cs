using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest Item", menuName = "Quest Item")]
public class QuestItem : ScriptableObject
{
    [SerializeField] public Item item;
    [SerializeField] int SpawnWithAmountToRetrive;
    int numToRetrieve;

    private void OnEnable()
    {
        numToRetrieve = SpawnWithAmountToRetrive;
    }

    public int GetNumToRetrieve()
    {
        return numToRetrieve;
    }

    public string GetItemName()
    {
        return item.itemName;
    }

    public Item GetItem()
    {
        return item;
    }

    public void DecrementNumToRetrieve()
    {
        numToRetrieve--;
    }
}
