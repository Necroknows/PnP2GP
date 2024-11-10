using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //Menu buttons, so that they can be turned off when loading the game
    public GameObject mainMenu;
    //Options for gameplay
    public GameObject optionsMenu;
    //the loading screen with progress bar
    public GameObject loadingScreen;
    public Image loadingBar;

    //this is a scene that contains the player, UI, GameManager, and the Directional light
    private string gameBasics = "GameBasics";

    //list of scenes to load at start of game
    private List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private void Start()
    {
        // make the cursor visible to interact with menus
        Cursor.visible = true;
        // keep the cursor in the play area 
        Cursor.lockState = CursorLockMode.Confined;
    }

    public void StartGame()
    {
        MainMenuHide();
        loadingScreenOn();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Village", LoadSceneMode.Additive));
        //scenesToLoad.Add(SceneManager.LoadSceneAsync("Farm", LoadSceneMode.Additive));
        scenesToLoad.Add(SceneManager.LoadSceneAsync(gameBasics));
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Farm", LoadSceneMode.Additive));
        StartCoroutine(LoadingScreen());
    }

    public void OptionSelect()
    {
        MainMenuHide();
        OptionMenuOn();
    }

    public void Back()
    {
        OptionMenuHide();
        MainMenuOn();
    }

    public void MainMenuHide()
    {
        mainMenu.SetActive(false);
    }

    public void MainMenuOn()
    {
        mainMenu.SetActive(true);
    }

    public void OptionMenuHide()
    {
        optionsMenu.SetActive(false);
    }
    public void OptionMenuOn()
    {
        optionsMenu.SetActive(true) ;
    }

    public void loadingScreenOn()
    {
        loadingScreen.SetActive(true);
    }
    public void exitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    IEnumerator LoadingScreen()
    {
        float progress = 0;
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                progress += scenesToLoad[i].progress;
                loadingBar.fillAmount = progress / scenesToLoad.Count;
                yield return null;
            }
        }
    }

}
