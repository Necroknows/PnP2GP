using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    int sceneIndexCurr;
    int sceneIndexNext;

    // --- Scene Loading --- 
    //this is a scene that contains the player, UI, GameManager, and the Directional light
    private string gameBasics = "GameBasics";

    //list of scenes to load at start of game
    private List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();


    public void resume()
    {
        UIManager.Instance.UnpauseGame();
    }

    public void restart()
    {
        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if(SceneManager.GetSceneAt(i).isLoaded == true && 
                    SceneManager.GetSceneByName(gameBasics) != SceneManager.GetSceneAt(i))
                SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            }
        }
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Village", LoadSceneMode.Additive));

        GameManager.instance.playerScript.spawnPlayerAtStart();
        UIManager.Instance.UnpauseGame();
    }
    public void nextLevel()
    {
    sceneIndexCurr = SceneManager.GetActiveScene().buildIndex; // get the index of the current scene
    sceneIndexNext= sceneIndexCurr+1;                          // index for the next scene is curr+1
        if(sceneIndexNext < SceneManager.sceneCountInBuildSettings)// if there is a next scene in the build 
        {
            
            SceneManager.LoadScene(sceneIndexNext); // load scene 
            UIManager.Instance.UnpauseGame();
        }
    }

    public void Respawn()
    {
        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i).isLoaded == true &&
                    SceneManager.GetSceneByName(gameBasics) != SceneManager.GetSceneAt(i))
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
                }
            }
        }

        //needs to be updated for the check point level to be the spawn location
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Village", LoadSceneMode.Additive));

        GameManager.instance.playerScript.spawnPlayer();
        UIManager.Instance.UnpauseGame();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        UIManager.Instance.UnpauseGame();
    }

    public void quit()
{
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

}

    // ---- Functionality for Buttons----

    IEnumerator WaitToLoad()
    {
        for (int i = 0; i < scenesToLoad.Count; i++)
        {
            while (!scenesToLoad[i].isDone)
            {
                
                yield return null;
            }
        }
    }
}
