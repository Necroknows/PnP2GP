/*
 * Author(s): Orion White
 * Date: 10-20-2024
 * Course: Full Sail University - Game Development Program
 * Project: Project and Portfolio 2
 * Description: 
 *     This script creates a CheckPoint based on the object it is attached to int the scene.
 *     The object this is attached to must have a trigger collider for it to work.
 *
 * Version: 1.0
 * 
 * Additional Notes:
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] bool isBedCheckpoint = false; //Bed bool, or not Bed bool.

    Color colorOriginal;

    // Start is called before the first frame update
    void Start()
    {
        colorOriginal = model.material.color;
    }
    //Edited so that player can trigger bed checkpoint or just a regular checkpoint.
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(isBedCheckpoint)
            {//Shows save button in UI.
                UIManager.Instance.EnableSaveButton();
            }
            else if (transform.position != GameManager.instance.playerSpawnPOS.transform.position)
            {
                SetCheckpoint();
            }
        }
    }
    //Hides save button when out of range.
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isBedCheckpoint)
        {
            UIManager.Instance.DisableSaveButton();
        }
    }
    //Updates playerSpawnPOS and gives feedback.
    void SetCheckpoint()
    {
        GameManager.instance.playerSpawnPOS.transform.position = transform.position;
        StartCoroutine(FlashColor());
    }

    IEnumerator FlashColor()
    {
        model.material.color = Color.red;
        UIManager.Instance.checkpointPopup.SetActive(true);  
        yield return new WaitForSeconds(1.0f);
        UIManager.Instance.checkpointPopup.SetActive(false);    
        model.material.color = colorOriginal;
    }
}
