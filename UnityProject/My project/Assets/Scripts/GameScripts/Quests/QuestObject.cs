using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]

public class QuestObject : ScriptableObject
{
    [SerializeField] string questName;

    [SerializeField] public List<QuestItem> questCollectables = new List<QuestItem>();
    QuestState state = QuestState.Inactive;
    bool justAccepted;
    public bool readyToTurnIn;

    private void OnEnable()
    {
        state = QuestState.Inactive;
        readyToTurnIn = false;
    }

    public string GetQuestName => questName;

    public List<QuestItem> GetQuestCollectables => questCollectables;

    public QuestState GetQuestState => state;

    public bool JustAccepted => justAccepted;

    public void SetJustAccepted(bool tf)
    {
        justAccepted = tf;
    }

    public void SetQuestState(QuestState _state)
    {
        state = _state;
    }

    public enum QuestState
    {
        Inactive,
        Accepted,
        Completed
    }
}
