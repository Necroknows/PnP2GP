using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private bool loadInProgress = false;
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
        //checks the load type, distance from transform or box collider
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
        //Debug.Log("checking distance from" + this.name);
        //if the distance is inside the load range and the scene is not loaded will call load scene
        if (Vector3.Distance(player.position, transform.position) < loadRange && !SceneManager.GetSceneByName(this.name).isLoaded && loadInProgress == false)
        { 
                    
                    StartCoroutine(LoadScene());
        }
        //if the distance of the player is outside of the load range and the scene is loaded calls unload scene
        else if (Vector3.Distance(player.position, transform.position) > loadRange && SceneManager.GetSceneByName(this.name).isLoaded && loadInProgress == false)
        {
            StartCoroutine(UnloadScene());
        }
       
    }

    IEnumerator LoadScene()
    {

       

        if (loadInProgress == false && !isLoaded)
        {
            //Debug.Log("Load Scene has started" + this.name);
            loadInProgress = true;
            //loads the scene
            if (!SceneManager.GetSceneByName(this.name).isLoaded)
            //waits for the scene to load
            {
                    SceneManager.LoadSceneAsync(gameObject.name, LoadSceneMode.Additive);
                    yield return WaitUntil.ReferenceEquals(SceneManager.GetSceneByName(this.name).isLoaded, true);
                    
                isLoaded = true;
                loadInProgress = false;
            }
            //Debug.Log("load scene has ended " + this.name);
        }
    }

   IEnumerator UnloadScene()
    {
        //Debug.Log("Unload Scene has started" + this.name);
        //checks if loading is in progress
        if (loadInProgress == false && isLoaded)
        {
            //check to make sure scene is infact loaded
            if (SceneManager.GetSceneByName(this.name).isLoaded)
            {
                //sets loadinginprogress bool to true
                loadInProgress = true;
                //until a scene is loaded 
                    SceneManager.UnloadSceneAsync(gameObject.name);
                    yield return WaitUntil.ReferenceEquals(!SceneManager.GetSceneByName(this.name).isLoaded, true);

                isLoaded = false;
                loadInProgress = false;
            }
        }
    }

    void TriggerCheck()
    {
        //can add checks if we want to add more types of scenes
    }
}
