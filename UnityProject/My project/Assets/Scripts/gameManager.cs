using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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
    public playerController playerScript;

    public bool isPaused;
    int enemyCount;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
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

    //public void statePause()
    //{
    //    isPaused = !isPaused;
    //    Time.timeScale = 0;
    //    Cursor.visible = true;
    //    Cursor.lockState = CursorLockMode.Confined;

    //}

    //public void stateUnpuase()
    //{
    //    isPaused = !isPaused;
    //    Time.timeScale = 1;
    //    Cursor.visible = false;
    //    Cursor.lockState = CursorLockMode.Locked;
    //    menuActive.SetActive(isPaused);
    //    menuActive = null;
    //}

    public void updateGameGoal(int amount)
    {
        enemyCount += amount;
        //enemyCountText.text = enemyCount.ToString("F0");
        if(enemyCount <= 0)
        {
            ////you win
            //statePause();
            //menuActive = menuWin;
            //menuActive.SetActive(isPaused);
            UIManager.Instance.ShowWinScreen();
        }
    }

    //public void youLose()
    //{
    //    statePause();
    //    menuActive = menuLose;
    //    menuActive.SetActive(isPaused);
    //}
}
