using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{
    int sceneIndexCurr;
    int sceneIndexNext;
    public void resume()
    {
        UIManager.Instance.UnpauseGame();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        GameManager.instance.playerScript.spawnPlayer();
        UIManager.Instance.UnpauseGame();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
        UIManager.Instance.UnpauseGame();
    }
    //Save to bed method.
    public void SaveToBed()
    {//Updates playerSpawnPOS to bed.
        GameManager.instance.playerSpawnPOS.transform.position = GameManager.instance.bedPosition;
     //Displays save confirmation.
        UIManager.Instance.checkpointPopup.SetActive(true);
        StartCoroutine(HideCheckpointPopupAfterDelay());
    }
    //Hides popup after delay.
    IEnumerator HideCheckpointPopupAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.checkpointPopup.SetActive(false);
    }

    public void quit()
{
#if UNITY_EDITOR
    UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif

}
}
