using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JOLCounter : MonoBehaviour
{



    private void OnTriggerEnter(Collider other)
    {
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            UIManager.Instance.goalUI.SetActive(true);
        }
       
    }

}
