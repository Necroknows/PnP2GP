using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]

public class QuestObject : ScriptableObject
{
    [SerializeField] string questName;

    [SerializeField] public List<QuestItem> questCollectables = new List<QuestItem>();

    
    public string GetQuestName()
    {
        return questName;
    }
    public List<QuestItem> GetList()
    {
        return questCollectables;
    }
}
