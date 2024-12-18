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

    Color colorOriginal;

    // Start is called before the first frame update
    void Start()
    {
        colorOriginal = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && transform.position != GameManager.instance.playerSpawnPOS.transform.position)
        {
            GameManager.instance.playerSpawnPOS.transform.position = transform.position;
            SavedPlayerState.instance.SaveCurrState();
            StartCoroutine(FlashColor());
        }
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
