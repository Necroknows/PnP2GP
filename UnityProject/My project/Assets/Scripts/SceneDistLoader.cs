using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum CheckMethod
{
    Distance,
    Trigger
}
public class SceneDistLoader : MonoBehaviour
{
    //player position
    public Transform player;
    //added incase we need to add trigger loading
    public CheckMethod checkMethod;
    //range between player and the scenes load transform
    public float loadRange;


    //scene state
    private bool isLoaded;
    

    private void Start()
    {
        //checks to make sure current scene isn't already loaded
        if (SceneManager.sceneCount > 0)
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (scene.name == gameObject.name)
                {
                    isLoaded = true;
                }
            }
        }
    }

    //checks for the distance or if player has entered a trigger 
    private void Update()
    {
        if (checkMethod == CheckMethod.Distance)
        {
            DistanceCheck();
        }
        else
        {
            TriggerCheck();
        }
    }

    //checks the distance between the player and the transform
    void DistanceCheck()
    {
        if (Vector3.Distance(player.position, transform.position) < loadRange)
        {
            LoadScene();
        }
        else
        {
            UnloadScene();
        }
    }


    //loads scene when player is in range
    void LoadScene()
    {
        if (!isLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            isLoaded = true;
        }
    }

    //unloads a scene when too far away
    void UnloadScene()
    {
        if (isLoaded)
        {
            SceneManager.UnloadSceneAsync(gameObject.name);
            isLoaded = false;
        }
    }

    void TriggerCheck()
    {
        //place holder incase we need to add trigger loading
    }
}
