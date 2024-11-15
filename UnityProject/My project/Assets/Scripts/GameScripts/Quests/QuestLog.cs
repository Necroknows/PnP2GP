using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestLog : MonoBehaviour
{
    List<QuestObject> questsToGive = new List<QuestObject>();
    List<QuestObject> questsAccepted = new List<QuestObject>();
    List<QuestObject> questsCompleted = new List<QuestObject>();
    QuestObject questActive = null;

    private void Update()
    {
        if (questActive != null)
        {
            QuestReadyToTurnIn(questActive);
        }
    }

    public bool QuestCheckIfCompleted (QuestObject obj)
    {
        if (questsCompleted != null)
        {
            if (questsCompleted.Contains(obj) && obj.GetQuestState == QuestObject.QuestState.Completed)
            {
                return true;
            }
            Debug.Log("Quest: " +  obj.GetQuestName + " was not found in questsCompleted list or was not marked as completed.");
        }
        else
        {
            Debug.Log("questsToGive is null.");
        }
        return false;
    }

    public bool QuestCheckIfStarted(QuestObject obj)
    {
        if (questsAccepted != null)
        {
            if (questsAccepted.Contains(obj) && obj.GetQuestState == QuestObject.QuestState.Accepted)
            {
                return true;
            }
            Debug.Log("Quest: " + obj.GetQuestName + " was not found in questsActive or was not marked as Active.");
        }
        else
        {
            Debug.Log("questsToGive is null.");
        }
        return false;
    }

    public bool QuestCheckIfQuestToGive(QuestObject obj)
    {
        if (questsToGive != null)
        {
            foreach (QuestObject quest in questsToGive)
            {
                if (obj.GetQuestName == quest.GetQuestName && quest.GetQuestState == QuestObject.QuestState.Inactive)
                {
                    return true;
                }
            }
        }
        else
        {
            Debug.Log("questsToGive is null.");
        }
        return false;
    }
    
    public void QuestComplete(QuestObject obj)
    {
        if (obj.GetQuestState == QuestObject.QuestState.Accepted && obj.readyToTurnIn)
        {
            foreach (QuestItem item in obj.GetQuestCollectables)
            {
                int i = InventoryManager.instance.Items.IndexOf(item.item);
                if (InventoryManager.instance.Items.Count > i && i > -1)
                {
                    InventoryManager.instance.Items[i].AddStack(item.GetNumToRetrieve * -1);
                }
            }
            // Set the next active quest
            QuestActiveNext();
            // Ensure the quest is in the appropriate List
            if (obj.GetQuestState == QuestObject.QuestState.Accepted || obj.GetQuestState == QuestObject.QuestState.Completed)
            {
                if (obj.GetQuestState == QuestObject.QuestState.Accepted)
                {
                    obj.SetQuestState(QuestObject.QuestState.Completed);
                    Debug.Log("Quest: " + obj.GetQuestName + " is now complete.");
                }
                else
                {
                    Debug.Log("Quest: " + obj.GetQuestName + " is already completed.");
                }
                if (!questsCompleted.Contains(obj))
                {
                    questsCompleted.Add(obj);
                }
                if (questsAccepted.Contains(obj))
                {
                    questsAccepted.Remove(obj);
                }
                if (questsToGive.Contains(obj))
                {
                    questsToGive.Remove(obj);
                }
            }

            else if (obj.GetQuestState == QuestObject.QuestState.Inactive)
            {
                Debug.Log("Quest: " + obj.GetQuestName + " has not been started.");
                if (!questsToGive.Contains(obj))
                {
                    questsToGive.Add(obj);
                }
                if (questsAccepted.Contains(obj))
                {
                    questsAccepted.Remove(obj);
                }
                if (questsCompleted.Contains(obj))
                {
                    questsCompleted.Remove(obj);
                }
            }
        }
    }

    public void QuestAccept(QuestObject obj)
    {
        if (obj != null)
        {
            if (obj.GetQuestState == QuestObject.QuestState.Accepted || obj.GetQuestState == QuestObject.QuestState.Inactive)
            {
                if (obj.GetQuestState == QuestObject.QuestState.Inactive)
                {
                    obj.SetQuestState(QuestObject.QuestState.Accepted);
                    Debug.Log("Quest: " + obj.GetQuestName + " is now accepted.");
                }
                else
                {
                    Debug.Log("Quest: " + obj.GetQuestName + " is already accepted.");
                }
                if (questActive == null)
                {
                    QuestSetActive(obj);
                }
                if (!questsAccepted.Contains(obj))
                {
                    questsAccepted.Add(obj);
                }
                if (questsToGive.Contains(obj))
                {
                    questsToGive.Remove(obj);
                }
            }

            else if (obj.GetQuestState == QuestObject.QuestState.Completed)
            {
                Debug.Log("Quest: " + obj.GetQuestName + " has already been completed.");
            }
            //Set the active quest as the new quest
            QuestSetActive(obj);
        }
    }

    public void QuestReadyToTurnIn(QuestObject obj)
    {
        bool tf = true;
        if (obj.GetQuestCollectables != null)
        {
            foreach (QuestItem item in obj.GetQuestCollectables)
            {
                if (!item.IsComplete)
                {
                    int i = InventoryManager.instance.Items.IndexOf(item.GetItem);
                    if (i != -1)
                    {
                        if (InventoryManager.instance.Items[i].GetStack >= item.GetItem.GetStack)
                        {
                            item.Complete();
                        }
                        else
                        {
                            tf = false;
                            break;
                        }
                    }
                    else
                    {
                        tf = false;
                        break;
                    }
                }
            }
            if (tf)
            {
                obj.readyToTurnIn = true;
            }
            else
            {
                obj.readyToTurnIn = false;
            }
        }
        else
        {
            Debug.Log("Null QuestObject passed to function QuestReadyToTurnIn().");
            obj.readyToTurnIn = false;
        }
    }

    public void QuestSetActive(QuestObject obj)
    {
        if (obj != null)
        {
            Debug.Log("Setting " + obj.GetQuestName + " as active quest.");
            if (obj.GetQuestState == QuestObject.QuestState.Completed)
            {
                Debug.Log("Quest: " + obj.GetQuestName + " has already been completed.");
            }
            else
            {
                questActive = obj;
            }
        }
    }

    public void QuestActiveNext()
    {
        if (questsAccepted.Count > 0)
        {
            int i = (questsAccepted.IndexOf(questActive) + 1) % questsAccepted.Count;
            questActive = questsAccepted[i];
        }
    }

    public void QuestActivePrev()
    {
        if (questsAccepted.Count > 0)
        {
            int i = (questsAccepted.IndexOf(questActive) - 1);
            if (i <= -1)
            {
                i = (questsAccepted.Count - 1);
            }
            questActive = questsAccepted[i];
        }
    }

    public QuestObject GetActiveQuest => questActive;

}
