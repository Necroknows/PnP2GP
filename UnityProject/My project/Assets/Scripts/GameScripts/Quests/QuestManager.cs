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
    [SerializeField] QuestObject questObject;


    List<QuestItem> questItems = new List<QuestItem>();

    bool HasAllQuestItems;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        //set the name of the quest, the list of items to collect
        questName.text = "Speak with Wizard";

        UpdateUIList();



    }

    public void Update()
    {

    }

    public void GiveQuest(QuestObject qObject)
    {
        if (questObject == null && !qObject.GetQuestCompleted())
        {
            questObject = qObject;
            questName.text = questObject.GetQuestName();
            questItems = questObject.GetList();
            UpdateUIList();
        }
    }


    public void UpdateUIList()
    {
        itemText.text = ""; //resets text
        itemAmount.text = "";
        int hasAmount;
        int needsAmount;
        bool hasItem = false;
        int hasAllItems = 0; //number of questitems that are in player inventory

        for (int eachItemQ = 0; eachItemQ < questItems.Count; eachItemQ++)
        {
            itemText.text += questItems[eachItemQ].GetItemName() + "\n";
            needsAmount = questItems[eachItemQ].GetNumToRetrieve();

            //checks between the quest list and the inventory list for each item and number of items needed
            for (int eachItemI = 0; eachItemI < InventoryManager.instance.Items.Count; eachItemI++)
            {
                if (questItems[eachItemQ].GetItemName() == InventoryManager.instance.Items[eachItemI].itemName)

                {
                    hasItem = true;
                    hasAmount = InventoryManager.instance.Items[eachItemI].GetStack;
                    if (needsAmount >= hasAmount && hasAmount > 0)
                    {
                        needsAmount = needsAmount - hasAmount;

                        itemAmount.text += needsAmount + 1 + "\n";
                    }
                    else
                    {
                        hasAllItems++; //adds to the has all items for each stack complete
                        itemAmount.text += "X" + "\n";
                    }

                }


            }
            if (!hasItem)
            {
                itemAmount.text += needsAmount + "\n";
                HasAllQuestItems = false;
            }
            hasItem = false;

        }
        //checks if the player has all the quest items
        if (hasAllItems == questItems.Count)
        {
            HasAllQuestItems = true;
        }

    }

    public bool CheckQuestComplete()
    {

        if (HasAllQuestItems)
        {
            for (int eachItemQ = 0; eachItemQ < questItems.Count; eachItemQ++)
            {
                for (int eachItemI = 0; eachItemI < InventoryManager.instance.Items.Count; eachItemI++)
                {

                    if (questItems[eachItemQ].GetItemName() == InventoryManager.instance.Items[eachItemI].itemName)
                    {
                        questItems[eachItemQ].DecrementNumToRetrieve();
                        InventoryManager.instance.RemoveItem(InventoryManager.instance.Items[eachItemI]);
                    }
                }
            }
            return true;
        }
        return false;

    }

    public void RemoveQuest()
    {
        if (questObject != null)
        {
            questObject.SetQuestComplete();
        }
        questName.text = string.Empty;
        questItems = null;
        questObject = null;
    }

    public QuestObject GetActiveQuest => questObject;
}