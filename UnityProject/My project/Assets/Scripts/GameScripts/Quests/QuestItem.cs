using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest Item", menuName = "Quest Item")]
public class QuestItem : ScriptableObject
{
    [SerializeField] public Item item;
    [SerializeField] int SpawnWithAmountToRetrive;
    int numObtained;
    int numToRetrieve;
    bool complete;

    private void OnEnable()
    {
        numToRetrieve = SpawnWithAmountToRetrive;
        numObtained = 0;
        complete = false;
    }

    public int GetNumToRetrieve => numToRetrieve;

    public int GetNumObtained => numObtained;

    public string GetItemName => item.itemName;

    public Item GetItem => item;

    public bool IsComplete => complete;

    public void Complete()
    {
        complete = true;
    }

    public void SetNumObtained(int num)
    {
        numObtained = num;
    }

    public void IncrementNumObtained()
    {
        numObtained++;
    }

    public void DecrementNumObtained()
    {
        numObtained--;
    }
}
