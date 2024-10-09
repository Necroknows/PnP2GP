using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [SerializeField] TMP_Text enemyCountText;
    public Image playerHpBar;
    public bool isPaused;

    float timescale;
    // Start is called before the first frame update
    void Awake()
    {
    
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {// on esc key 
        if (Input.GetButtonDown("Cancel"))
        {
            Debug.Log("Escape key pressed");
            // if no menu
            if (menuActive == null)
            {
                PauseGame();


            }
            else if (menuActive !=null)
            {
                UnpauseGame();

            }
        }
        int count= GameManager.instance.GetEnemyCount();
        enemyCountText.text = count.ToString();
    }
    public void PauseGame()
    {
        // place hold the original timescale
        timescale = Time.timeScale;
        //set current timescale to 0
        Time.timeScale = 0;
        // make the cursor visible to interact with menus
        Cursor.visible = true;
        // keep the cursor in the play area 
        Cursor.lockState = CursorLockMode.Confined;
        // pause state toggled to true
        isPaused = true;
        menuActive = menuPause;
        menuActive.SetActive(true);
    }
    public void UnpauseGame()
    { // return to original timescale
        Time.timeScale = timescale;
        // cursor off 
        Cursor.visible = false;
        // Lock cursor 
        Cursor.lockState = CursorLockMode.Locked;
        // toggle pause state on
        isPaused = false;
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
    }
    public void updateEnemyCount(int count)
    {
        // updated enmey count to display to tracker 
        enemyCountText.text = count.ToString();
    }
    public void UPdatePlayerHealthBar(float healthFraction)
    {
        // fills per fraction of health 
        playerHpBar.fillAmount = healthFraction;
    }
    public void ShowWinScreen()
    {
        // pause game 
        PauseGame();
        // you win 
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }

        menuActive = menuWin;
        menuActive.SetActive(true);

    }
    public void ShowLoseScreen()
    {
        // pause game 
        PauseGame();
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
        // you lose 
        menuActive = menuLose;
        menuActive.SetActive(true);

    }
    
}
