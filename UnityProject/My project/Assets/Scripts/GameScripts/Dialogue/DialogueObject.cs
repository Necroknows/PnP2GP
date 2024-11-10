/*
 * 
 * Made By Orion White
 * 
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueObject")]
public class DialogueObject : ScriptableObject
{


    public string nameNPC;

    [SerializeField] [TextArea] private string[] dialogue;
    [SerializeField] private Response[] responses;
    [SerializeField] public QuestObject questToGive;


    public bool HasResponses => responses != null && responses.Length > 0;
    public string[] Dialogue => dialogue;
    public Response[] Responses => responses;




}
