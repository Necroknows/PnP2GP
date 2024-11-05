using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AreaMessageTrigger : MonoBehaviour
{//Displays message if player exits the scripted objects collider.
    [SerializeField] private string areaMessage = "Haunted Forest";
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI lineText;
    [SerializeField] private Animator anim;
    [SerializeField] private float textSpeed = 0.05f;

    private Queue<string> lines;
    private bool messageDisplayed = false;

    private void Awake()
    {
        lines = new Queue<string>();
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && !messageDisplayed)
        {
            lines.Clear();
            lines.Enqueue(areaMessage);
            ShowAreaMessage();
            messageDisplayed = true;//Ensures message won't display more than once.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            EndDialogue();
            messageDisplayed = false;//Resets message state to trigger upon exit again.
        }
    }

    private void ShowAreaMessage()
    {//Uses a trigger to display an area location name.
        anim.SetTrigger("ShowLocationText");
        nameText.text = areaMessage;

        DisplayNextSentence();
    }

    private void DisplayNextSentence()
    {
        if(lines.Count == 0)
        {//Ends if no more text.
            EndDialogue();
            return;
        }

        string line = lines.Dequeue();//Grabs next line in queue.
        StopAllCoroutines();
        StartCoroutine(TypeLine(line));
    }

    private IEnumerator TypeLine(string line)
    {
        lineText.text = string.Empty;
        foreach (char c in line.ToCharArray())
        {
            lineText.text += c; //Appends the characters.
            yield return new WaitForSeconds(textSpeed);//Wait for specified time.
        }
    }

    private void EndDialogue()
    {
        anim.SetTrigger("HideLocationText");
    }
}
