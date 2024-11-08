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
    [SerializeField] TMP_Text ammoCountText;
    
   
    //public GameObject goalUI;
    public GameObject checkpointPopup;
    public PlayerController playerCont;

    //=== PLAYER INVENTORY ===
    public GameObject inventoryPanel;           //sets inventory panel
    public KeyCode interactKey = KeyCode.E;       //sets key for interact
    //private bool isInventoryOpen = false;       //checks if inventory is open
    
    //public Image pumpkinFill;
    public Image playerHpBar;
    public Image playerFuelBar;
    public bool isPaused;

    float timescale;
    // Start is called before the first frame update
    void Awake()
    {
        timescale=Time.timeScale;
        Instance = this;
    }
    private void Start()
    {
        playerCont=FindObjectOfType<PlayerController>();
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
            else if (menuActive != null && menuActive == menuPause)
            {
                UnpauseGame();
            }
        }
        
        
        if(playerCont !=null)
        {
            int ammoCount = playerCont.GetAmmo();
            ammoCountText.text = ammoCount.ToString();
           
        }

        //on E key
        
        
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

    
    
    public void UpdatePlayerHealthBar(float healthFraction)
    {
        healthFraction = Mathf.Clamp01(GameManager.instance.playerScript.getHP()/ GameManager.instance.playerScript.getHPOrig());
        // fills per fraction of health 
        playerHpBar.fillAmount = healthFraction;
    }
    
    //public void UpdatePumpkinFill()
    //{
    //    // Get the current player's score and goal score
    //    float currentScore = GameManager.instance.GetPlayerScore();
    //    float goalScore = GameManager.instance.GetGoalScore();

    //    // Calculate the fill fraction based on current score divided by the goal score
    //    float fillFraction = Mathf.Clamp01(currentScore / goalScore);  // Clamped between 0 and 1

    //    // Update the fill amount for the pumpkin UI element
    //    pumpkinFill.fillAmount = fillFraction;  // Directly set the fill amount
    //}
    
    public void UpdatePlayerFuelBar(float fuelFraction)
    {
        fuelFraction = Mathf.Clamp01(GameManager.instance.playerScript.GetFuel() / GameManager.instance.playerScript.GetFuelMax());
        //Fill the FuelCan icon by fraction of fuel
       playerFuelBar.fillAmount = fuelFraction;
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
    
    public void CloseMenu()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false);              // Deactivate the currently active menu
            menuActive = null;                        // Clear the active menu reference
        }
    }
   

   
}
//END