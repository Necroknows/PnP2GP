using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class ResponseHandler : MonoBehaviour
{
    [SerializeField] private RectTransform responseBox;
    [SerializeField] private RectTransform responseButtonTemplate;
    [SerializeField] private RectTransform responseContainer;
    private InventoryManager inventory;
    private DialogueManager dialogueManager;

    List<GameObject> tempResponseButtons = new List<GameObject>();
    List<QuestItem> itemsToRemove = new List<QuestItem>();

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        inventory = FindObjectOfType<InventoryManager>();
        responseBox.gameObject.SetActive(false);

        if (inventory == null)
        {
            Debug.Log("Inventory not found");
        }
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;

        foreach (Response response in responses)
        {
            if (CheckForQuests(response))
            {
                GameObject responseButton = Instantiate(responseButtonTemplate.gameObject, responseContainer);
                responseButton.gameObject.SetActive(true);
                responseButton.GetComponent<TMP_Text>().text = response.ResponseText;
                responseButton.GetComponent<Button>().onClick.AddListener(() => OnPickedResponse(response));

                tempResponseButtons.Add(responseButton);

                responseBoxHeight += responseButtonTemplate.sizeDelta.y;
            }
        }

        responseBox.sizeDelta = new Vector2(responseBox.sizeDelta.x, responseBoxHeight);
        responseBox.gameObject.SetActive(true);
        if (tempResponseButtons.Count > 0)
        {
            EventSystem.current.SetSelectedGameObject(tempResponseButtons[0]);
        }
    }

    private void OnPickedResponse(Response response)
    {
        bool hasItem = false;
        responseBox.gameObject.SetActive(false);

        foreach (GameObject responseButton in tempResponseButtons)
        {
            Destroy(responseButton);
        }
        tempResponseButtons.Clear();

        if (QuestManager.instance.GetActiveQuest != null)
        {
            if (QuestManager.instance.CheckQuestComplete())
            {
                hasItem = true;
            }
        }

        if (hasItem && response.DialogueTrue != null)
        {
            QuestManager.instance.RemoveQuest();
            dialogueManager.StartDialogue(response.DialogueTrue);
        }
        else if (response.DialogueTrue != null && response.DialogueFalse == null)
        {
            dialogueManager.StartDialogue(response.DialogueTrue);
        }

        else if (!hasItem && response.DialogueFalse != null)
        {
            dialogueManager.StartDialogue(response.DialogueFalse);
        }

        else
        {
            Debug.Log("No dialogue suitable for conversation outcome.");
        }

    }

    private bool CheckForQuests(Response response)
    {
        if (response.requiredQuestsCompleted.Length > 0)
        {
            foreach (QuestObject thing in response.requiredQuestsCompleted)
            {
                if (thing != null)
                {
                    Debug.Log("Checking Completed Quests for: " + thing.GetQuestName());
                    if (!thing.GetQuestCompleted())
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
