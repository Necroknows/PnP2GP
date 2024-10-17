using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    // MOVED TO UI SCRIPT 
    //[SerializeField] GameObject menuActive;
    //[SerializeField] GameObject menuPause;
    //[SerializeField] GameObject menuWin;
    //[SerializeField] GameObject menuLose;
    //[SerializeField] TMP_Text enemyCountText;
    public Image playerHpBar;

    public GameObject flashDamageScreen;

    public GameObject player;
    public GameObject pumpkin;
    public playerController playerScript;

    public bool isPaused;
    int enemyCount;
    int retrievableCount;

    //list for retrievable objects
    List<RetrievableObjects> retrievableObjects = new List<RetrievableObjects>();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();

        //fill list with all retrievable objects in scene
        fillRetrievables();     //<---can be moved elsewhere if need be
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetButtonDown("Cancel"))
        //{
        //    if (menuActive == null)
        //    {
        //        statePause();
        //        menuActive = menuPause;
        //        menuActive.SetActive(isPaused);

        //    }
        //    else if (menuActive == menuPause)
        //    {
        //        stateUnpuase();
        //    }
        //}
    }

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        if(enemyCount <= 0)
        {
            ////you win
            //statePause();
            //menuActive = menuWin;
            //menuActive.SetActive(isPaused);
            UIManager.Instance.ShowWinScreen();
        }
    }

    public int GetEnemyCount()
        {return enemyCount; }

    //fill list with all retrievable objects in scene
    public void fillRetrievables()
    {
        //find all retrievableobject components in scene & add to list
        RetrievableObjects[] allRetrievables = FindObjectsOfType<RetrievableObjects>();
        
        //fill list w/ objects
        foreach (RetrievableObjects retrievable in allRetrievables)
        {
            retrievableObjects.Add(retrievable);
        }    
    }

    //handles retrievable of an object
    public void RetrieveObject(RetrievableObjects retrievable)
    {
        if (!retrievable.isRetrieved)
        {
            //mark as retrieved
            retrievable.Retrieve();

            //update count of retrievable objects
            updateRetrievableCount();
        }
    }

    //update the count of retrievable objects
    public int updateRetrievableCount()
    {
        return retrievableObjects.Count;
    }

    //resets all objects for restarting game/level
    public void ResetAllObjects()
    {
        foreach (var retrievable in retrievableObjects)
        {
            retrievable.ResetObject();
        }
    }
}
