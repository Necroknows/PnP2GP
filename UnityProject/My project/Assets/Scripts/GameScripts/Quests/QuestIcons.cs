using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QuestIcons : MonoBehaviour
{
    [SerializeField] GameObject QuestAvailable;
    [SerializeField] GameObject QuestTurnIn;


    public float hoverHeight = 0.5f;
    public float hoverSpeed = 3.0f;
    public float hoverRange = 0.5f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        QuestTurnIn.SetActive(false);
        QuestAvailable.SetActive(true);

    }

    void Update()
    {
        if (GameManager.instance != null)
        {
            SwapQuestIcons();
        }
        float newY = startPos.y + Mathf.Sin(Time.time * hoverSpeed) * (hoverRange / 2);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        if (GameManager.instance != null)
        {
            this.transform.LookAt(GameManager.instance.player.transform.position);
        }
    }

    void SwapQuestIcons()
    {
        if (GameManager.instance.playerScript.QuestLog.GetActiveQuest != null && 
            GameManager.instance.playerScript.QuestLog.GetActiveQuest.readyToTurnIn == true)
        {
            QuestAvailable.SetActive(false);
            QuestTurnIn.SetActive(true);
        }
        else if (GameManager.instance.playerScript.QuestLog.GetActiveQuest == null)
        {
            QuestAvailable.SetActive(true);
            QuestTurnIn.SetActive(false);
        }
        else
        {
            QuestAvailable.SetActive(false);
            QuestTurnIn.SetActive(false);
        }
    }
}
