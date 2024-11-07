using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SceneLoadType
{
    Distance,
    Trigger
}


public class SceneLoader : MonoBehaviour
{

    public Transform player;
    public SceneLoadType checkMethod;
    public float loadRange;


    //scene state
    private bool isLoaded;

    private void Start()
    {
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
    private void Update()
    {
        if (checkMethod == SceneLoadType.Distance)
        {
            DistanceCheck();
        }
        else
        {
            TriggerCheck();
        }
    }

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

    void LoadScene()
    {
        if (!isLoaded)
        {
            SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
            isLoaded = true;
        }
    }

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
        //can add checks if we want to add more types of scenes
    }
}
