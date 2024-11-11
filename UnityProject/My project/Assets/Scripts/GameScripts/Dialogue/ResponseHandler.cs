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
        itemsToRemove = dialogueManager.dialogueObject.questToGive.questCollectables;

        foreach (Response response in responses)
        {
            if (CheckForItems(response))
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

        if (CheckForItems(response))
        {
            hasItem = true;
        }

        if (hasItem && response.DialogueTrue != null)
        {
            // Complete quest function here
            dialogueManager.StartDialogue(response.DialogueTrue);
            // Give next quest function here
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

    private bool CheckForItems(Response response)
    {

        if (itemsToRemove.Count > 0)
        {
            foreach (QuestItem thing in itemsToRemove)
            {
                if (thing != null)
                {
                    Debug.Log("Checking Inventory for: " + thing.item.itemName);
                    if (!inventory.HasItem(thing.item))
                    {
                        return false;
                    }
                }
            }
        }
        if (itemsToRemove.Count > 0)
        {
            foreach (QuestItem thing in itemsToRemove)
            {
                if (thing != null)
                {
                    Debug.Log("Checking Inventory for: " + thing.item.itemName);
                    if (inventory.HasItem(thing.item))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}
