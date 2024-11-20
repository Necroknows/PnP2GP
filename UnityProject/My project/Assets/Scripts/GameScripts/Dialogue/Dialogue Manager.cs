/*
 * 
 * Made By Orion White
 * 
 * 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] float textSpeed;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI lineText;
    [SerializeField] private GameObject hUD;
    [SerializeField] AudioClip textSound;

    private float textSpeedOriginal;
    private AudioSource textSourceAudio;
    private PlayerController controller;
    private ResponseHandler responseHandler;
    public DialogueObject dialogueObject;
    private bool isShowingResponses;

    public Animator anim;

    Queue<string> lines;

    // Start is called before the first frame update
    void Awake()
    {
        lines = new Queue<string>();
        instance = this;
    }

    private void Start()
    {
        controller = FindObjectOfType<PlayerController>();
        responseHandler = FindObjectOfType<ResponseHandler>();
        textSourceAudio = gameObject.AddComponent<AudioSource>();
        textSourceAudio.loop = false;
        textSourceAudio.clip = textSound;
        textSpeedOriginal = textSpeed;
        instance.gameObject.SetActive(true);
    }

    private void Update()
    {
        // If the player presses Backspace while the dialogue is open, close it and free the player
        if (anim.GetBool("IsOpen") == true && Input.GetKeyUp(KeyCode.Space))
        {
            DisplayNextSentence();
        }
        else if (anim.GetBool("IsOpen") == true && Input.GetKeyUp(KeyCode.Backspace))
        {
            StartCoroutine(EndDialogue());
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            textSpeed /= 2;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            textSpeed = textSpeedOriginal;
        }
    }

    public void StartDialogue(DialogueObject dialogue)
    {
        Debug.Log("Starting conversation with " + dialogue.nameNPC);

        hUD.SetActive(false);
        anim.SetBool("IsOpen", true);
        isShowingResponses = false;

        nameText.text = dialogue.nameNPC;
        dialogueObject = dialogue;
        lines.Clear();

        foreach (string line in dialogue.Dialogue)
        {
            lines.Enqueue(line);
        }

        DisplayNextSentence();

    }

    public void DisplayNextSentence()
    {
        if (lines.Count == 0 && dialogueObject.HasResponses)
        {
            if (!isShowingResponses)
            {
                responseHandler.ShowResponses(dialogueObject.Responses);
                isShowingResponses = true;
            }
            if (dialogueObject.questToGive != null)
            {
                dialogueObject.questToGive.SetJustAccepted(true);
                QuestManager.instance.GiveQuest(dialogueObject.questToGive);
            }
        }
        else if (lines.Count == 0)
        {
            if (dialogueObject.questToGive != null)
            {
                dialogueObject.questToGive.SetJustAccepted(true);
                QuestManager.instance.GiveQuest(dialogueObject.questToGive);
            }
            StartCoroutine(EndDialogue());
            return;
        }
        else
        {
            string line = lines.Dequeue();
            StopAllCoroutines();
            lineText.text = string.Empty;
            StartCoroutine(TypeLine(line));
            //lineText.text = line;
        }
    }

    public IEnumerator EndDialogue()
    {
        Debug.Log("End of conversation.");
        if (QuestManager.instance.GetActiveQuest != null && QuestManager.instance.GetActiveQuest.JustAccepted)
        {
            QuestManager.instance.GetActiveQuest.SetJustAccepted(false);
        }
        responseHandler.DestroyResponses();
        StopAllCoroutines();
        anim.SetBool("IsOpen", false);
        controller.enabled = true;
        hUD.SetActive(true);
        yield return new WaitForSeconds(1);
        if (dialogueObject != null && dialogueObject.isWinDialogue)
        {
            instance.gameObject.SetActive(false);
            UIManager.Instance.ShowWinScreen();
        }
        
    }

    IEnumerator TypeLine(string line)
    {
        foreach (char c in line.ToCharArray())
        {
            lineText.text += c;
            textSourceAudio.Play();
            yield return new WaitForSeconds(textSpeed);
        }
    }
}