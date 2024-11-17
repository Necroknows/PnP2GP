using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;


public class QuestManager : MonoBehaviour
{
    public static QuestManager instance;

    [SerializeField] TMP_Text questName;
    [SerializeField] TMP_Text itemText;
    [SerializeField] TMP_Text itemAmount;
    [SerializeField] QuestObject activeQuest;
    PlayerController playerScript;

    List<QuestItem> activeQuestCollectables = new List<QuestItem>();

    bool hasAllQuestItems;
    public bool HasAllQuestItems => hasAllQuestItems;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        //set the name of the quest, the list of items to collect
        questName.text = "Speak with Witch";
        itemText.text = " ";
        itemAmount.text = " ";
        playerScript = GameManager.instance.playerScript;
        UpdateUIList();
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.Y))
        {
            playerScript.QuestLog.QuestActiveNext();
        }
        else if (Input.GetKeyUp(KeyCode.T))
        {
            playerScript.QuestLog.QuestActivePrev();
        }
    }

    public void GiveQuest(QuestObject qObject)
    {
        if (qObject != null && qObject.GetQuestState == QuestObject.QuestState.Inactive)
        {
            hasAllQuestItems = false;
            qObject.readyToTurnIn = false;
            qObject.SetQuestState(QuestObject.QuestState.Inactive);
            playerScript.QuestLog.QuestAccept(qObject);
            UpdateUIList();
            Debug.Log("Giving Quest: " + qObject.GetQuestName);
        }
        else
        {
            Debug.Log("Quest: " + qObject.GetQuestName + " has already started");
        }
    }
   
    
    public void UpdateUIList()
    {
        if (playerScript != null && playerScript.QuestLog != null && playerScript.QuestLog.GetActiveQuest != null)
        {
            activeQuest = playerScript.QuestLog.GetActiveQuest;
        }
        if (activeQuest != null)
        {
            questName.text = activeQuest.GetQuestName;
            itemText.text = ""; //resets text
            itemAmount.text = "";
            int hasAllItems = 0; //number of questitems that are in player inventory

            foreach (QuestItem collectable in activeQuest.GetQuestCollectables)
            {
                itemText.text += collectable.GetItemName + "\n";

                //checks if the item numToRetrieve is fufilled 
                if (collectable.IsComplete)
                {
                    hasAllItems++; //adds to the has all items for each stack complete
                    itemAmount.text += "X" + "\n";
                }
                else
                {
                    itemAmount.text += (collectable.GetNumToRetrieve - collectable.GetNumObtained) + "\n";
                }
            }
        }
        else
        {
            Debug.Log("activeQuest was null.");
        }
    }


    public void RemoveQuest()
    {
        if (activeQuest.readyToTurnIn)
        {
            playerScript.QuestLog.QuestComplete(activeQuest);
        }
        questName.text = string.Empty;
        activeQuestCollectables = null;
        activeQuest = null;
    }

    public QuestObject GetActiveQuest => activeQuest;
}