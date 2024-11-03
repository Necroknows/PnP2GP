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

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;

    [SerializeField] float textSpeed;

    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI lineText;
    PlayerController controller;

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
    }

    private void Update()
    {
        // If the player presses Backspace while the dialogue is open, close it and free the player
        if (anim.GetBool("IsOpen") == true && Input.GetKeyUp(KeyCode.E))
        {
            DisplayNextSentence();
        }
        else if (anim.GetBool("IsOpen") == true && Input.GetKeyUp(KeyCode.Backspace))
        {
            EndDialogue();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        Debug.Log("Starting conversation with " + dialogue.name);

        anim.SetBool("IsOpen", true);

        nameText.text = dialogue.name;

        lines.Clear();

        foreach (string line in dialogue.lines)
        {
            lines.Enqueue(line);
        }

        DisplayNextSentence();

    }

    public void DisplayNextSentence()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        string line = lines.Dequeue();
        StopAllCoroutines();
        lineText.text = string.Empty;
        StartCoroutine(TypeLine(line));
        //lineText.text = line;
    }

    public void EndDialogue()
    {
        Debug.Log("End of conversation.");
        StopAllCoroutines();
        anim.SetBool("IsOpen", false);
        controller.enabled = true;
    }

    IEnumerator TypeLine(string line)
    {
        foreach (char c in line.ToCharArray())
        {
            lineText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }
}
