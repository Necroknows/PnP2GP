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
    [SerializeField] private Inventory inventory;
    private DialogueManager dialogueManager;
    private EventSystem eventSystem;

    List<GameObject> tempResponseButtons = new List<GameObject>();

    private void Start()
    {
        dialogueManager = GetComponent<DialogueManager>();
        eventSystem = FindObjectOfType<EventSystem>();
    }

    public void ShowResponses(Response[] responses)
    {
        float responseBoxHeight = 0;
        
        foreach (Response response in responses)
        {
            bool hasItem = true;
            if (response.thingsToCheck != null)
            {
                foreach (Item thing in response.thingsToCheck)
                {
                    if (!inventory.HasItem(thing))
                    {
                        hasItem = false;
                    }
                }
            }
            if (response.thingsToIgnore != null)
            {
                foreach (Item thing in response.thingsToIgnore)
                {
                    if (inventory.HasItem(thing))
                    {
                        hasItem = false;
                    }
                }
            }
            if (hasItem)
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
        EventSystem.current.SetSelectedGameObject(tempResponseButtons[0]);
    }

    private void OnPickedResponse(Response response)
    {
        bool hasItem = true;
        if (response.thingsToCheck != null)
        {
            foreach (Item thing in response.thingsToCheck)
            {
                if (!inventory.HasItem(thing))
                {
                    hasItem = false;
                }
            }
        }
        if (response.thingsToIgnore != null)
        {
            foreach (Item thing in response.thingsToIgnore)
            {
                if (inventory.HasItem(thing))
                {
                    hasItem = false;
                }
            }
        }
        responseBox.gameObject.SetActive(false);

        foreach (GameObject responseButton in tempResponseButtons)
        {
            Destroy(responseButton);
        }
        tempResponseButtons.Clear();

        if (hasItem && response.DialogueTrue != null)
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
}
