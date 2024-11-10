using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{


    [SerializeField] TMP_Text questName;
    [SerializeField] TMP_Text itemText;
    [SerializeField] TMP_Text itemAmount;
    [SerializeField] QuestObject questObject;
    

    List<QuestItem> questItems = new List<QuestItem>();
    

    private void Update()
    {

    }
    public void Start()
    {
        //set the name of the quest, the list of items to collect
        questName.text = questObject.GetQuestName();
        questItems = questObject.GetList();
        UpdateUIList();
        
        

    }


    public void UpdateUIList()
    {
        itemText.text = "";
        itemAmount.text = "";
        int hasAmount;
        int needsAmount;
        bool hasItem = false;


        for (int eachItem = 0; eachItem < questItems.Count; eachItem++)
        {
            itemText.text += questItems[eachItem].GetItemName() + "\n";
            needsAmount = questItems[eachItem].GetNumToRetrieve();
            for (int eachItem2 = 0; eachItem2 < InventoryManager.instance.Items.Count; eachItem2++)
            {
                
                if (questItems[eachItem].GetItemName() == InventoryManager.instance.Items[eachItem2].itemName)
                {
                    hasItem = true;
                    hasAmount = InventoryManager.instance.Items[eachItem2].GetStack;
                    

                    if (needsAmount >= hasAmount && hasAmount > 0)
                    {
                        needsAmount = needsAmount - hasAmount;
                        
                        itemAmount.text += needsAmount + "\n";
                    }
                    else
                    {
                        
                        itemAmount.text += "X" + "\n";
                    }
                    
                }


            }
            if(!hasItem) 
            {
                itemAmount.text += needsAmount + "\n";
            }
            hasItem = false;

        }
       
    }

}
