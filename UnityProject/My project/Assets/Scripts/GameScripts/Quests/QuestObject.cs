
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]

public class QuestObject : ScriptableObject
{
    [SerializeField] string questName;

    [SerializeField] public List<QuestItem> questCollectables = new List<QuestItem>();
    bool isQuestCompleted = false;

    private void OnEnable()
    {
        if (isQuestCompleted)
        {
            isQuestCompleted = false;
        }
    }


    public string GetQuestName()
    {
        return questName;
    }
    public List<QuestItem> GetList()
    {
        return questCollectables;
    }

    public void SetQuestComplete()
    {
        isQuestCompleted = true;
    }
    public bool GetQuestCompleted()
    {
        return isQuestCompleted;
    }
}
