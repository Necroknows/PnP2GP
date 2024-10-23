using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;



public class InfoMans : MonoBehaviour
{

    // Fields to set canvas 
    [SerializeField] GameObject tutorialPopUp;
    [SerializeField] TMP_Text tutorialText_T;

    [SerializeField] GameObject infoMan;

    //find out why this enum will not show up in inspector
    //[SerializeField] public enum InfoManType { StaticMans, AnimatedMans };
    //InfoManType type;

    [SerializeField] bool isAnimated;
    [SerializeField] Animator anim;


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
            if (isAnimated)
            { anim.SetTrigger("PlayerEnter"); }

        }
    }
    private void OnTriggerExit(Collider other)
    {
        HideTutorial();
        if(isAnimated)
        {
            anim.SetTrigger("PlayerLeave");
        }
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
