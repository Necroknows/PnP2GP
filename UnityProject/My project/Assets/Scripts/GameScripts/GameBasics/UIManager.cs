using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    public static UIManager Instance;
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuOptions;
    [SerializeField] GameObject menuHowTo;
    [SerializeField] GameObject deathSpawnWarning;
    [SerializeField] GameObject infoContainer;
    [SerializeField] GameObject hUD;
    [SerializeField] TMP_Text ammoCountText;
    [SerializeField] GameObject OpeningScene;

    Animation anim;
    
   
    //public GameObject goalUI;
    public GameObject checkpointPopup;
    public PlayerController playerCont;
    public DeathAI deathAI;

    //=== PLAYER INVENTORY ===
    public GameObject inventoryPanel;           //sets inventory panel
    public KeyCode interactKey = KeyCode.E;       //sets key for interact
                                                  //private bool isInventoryOpen = false;       //checks if inventory is open

    public Image deathBar;
    public Image playerHpBar;
    public Image playerFuelBar;
    public bool isPaused;
    bool isOpeningActive;
    bool isWinMenuActive;

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
        StartOpeningScene();
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
        if(Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Mouse0) && isOpeningActive)
        {
            CloseOpeningScene();
        }
        
        if(playerCont !=null)
        {
            int ammoCount = playerCont.GetAmmo();
            ammoCountText.text = ammoCount.ToString();
           
        }

        //on E key
        
        //Death awareness update
        DeathSpawnBar();
    }

    void StartOpeningScene()
    {
        timescale = Time.timeScale;
        //set current timescale to 0
        Time.timeScale = 0;
        // make the cursor visible to interact with menus
        Cursor.visible = true;
        // keep the cursor in the play area 
        Cursor.lockState = CursorLockMode.Confined;
        isOpeningActive = true;
        OpeningScene.SetActive(true);
        menuActive = OpeningScene;
    }

    public void CloseOpeningScene()
    {
        // return to original timescale
        Time.timeScale = 1;
        // cursor off 
        Cursor.visible = false;
        // Lock cursor 
        Cursor.lockState = CursorLockMode.Locked;
        // toggle pause state on
        menuActive = null;
        OpeningScene.SetActive(false);
        hUD.SetActive(true);
        isPaused = false;
        isOpeningActive = false;
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
        hUD.SetActive(false);
        infoContainer.SetActive(false);
    }
    
    public void UnpauseGame()
    {
        if (isPaused)
        {
            // return to original timescale
            Time.timeScale = 1;
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
 
            if(hUD != null) hUD.SetActive(true);
            if(infoContainer != null) infoContainer.SetActive(true);
            if(playerCont != null) playerCont.enabled = true;

            AudioListener.pause = false;
        }
    }
     public void OptionsForGame()
    {
        menuActive.SetActive(false);
        menuActive = menuOptions;
        menuActive.SetActive(true);

    }

    public void Back()
    {
        menuActive.SetActive(false);
        menuActive = menuPause;
        menuActive.SetActive(true);
    }

    public void HowToCraft()
    {
        menuActive.SetActive(false);
        menuActive = menuHowTo;
        menuActive.SetActive(true);
    }


    public void UpdatePlayerHealthBar(float healthFraction)
    {
        healthFraction = Mathf.Clamp01(GameManager.instance.playerScript.getHP()/ GameManager.instance.playerScript.getHPOrig());
        // fills per fraction of health 
        playerHpBar.fillAmount = healthFraction;
    }

    public void UpdateAmmoCount(int ammo)
    {
        ammoCountText.text = ammo.ToString(); //Separate method to update ammo count on respawn.
    }
    
    public void DeathSpawnBar()
    {
            //fill bar based on the percentage between the two

            //update the bar to the percentage
            deathBar.fillAmount = GameManager.instance.dSManager.GetCurrentFillAmount();
            if (deathBar.fillAmount == 1 && !deathSpawnWarning.activeInHierarchy)
            {
                deathSpawnWarning.SetActive(true);
            }
            else if (deathBar.fillAmount < 1 && deathSpawnWarning.activeInHierarchy)
            { deathSpawnWarning.SetActive(false); }

    }

    public void UpdatePlayerFuelBar(float fuelFraction)
    {
        fuelFraction = Mathf.Clamp01(GameManager.instance.playerScript.GetFuel() / GameManager.instance.playerScript.GetFuelMax());
        //Fill the FuelCan icon by fraction of fuel
       playerFuelBar.fillAmount = fuelFraction;
    }
    
    public void ShowWinScreen()
    {
        SceneManager.LoadScene("Credits");

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
   
    public void checkpointUILoad(int ammo, float healthFraction)
    {
        UpdateAmmoCount(ammo);
        UpdatePlayerHealthBar(healthFraction);
    }
   
}
//END