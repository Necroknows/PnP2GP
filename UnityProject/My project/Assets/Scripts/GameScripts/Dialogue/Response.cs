using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Response
{
    [SerializeField] public Item[] thingsToCheck;
    [SerializeField] public Item[] thingsToIgnore;
    [SerializeField] private string responseText;
    [SerializeField] private DialogueObject dialogueTrue;
    [SerializeField] private DialogueObject dialogueFalse;

    public DialogueObject DialogueTrue => dialogueTrue;
    public DialogueObject DialogueFalse => dialogueFalse;
    public string ResponseText => responseText;
}
