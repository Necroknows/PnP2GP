using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InfoMans : MonoBehaviour
{
    // Fields to set canvas 
    [SerializeField] GameObject tutorialPopUp;
    [SerializeField] TMP_Text tutorialText_T;
    


    // Start is called before the first frame update
    void Start()
    {
        // pop up is hidden on start 
        tutorialPopUp.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowTutorial();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        HideTutorial();
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void ShowTutorial()
    {
        tutorialPopUp.SetActive(true);
    }
    public void HideTutorial()
    {
        tutorialPopUp.SetActive(false);
    }
}
