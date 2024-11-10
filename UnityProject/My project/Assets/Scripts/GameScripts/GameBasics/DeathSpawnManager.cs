using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.AI;
// ---Death spawn script---
//holds counter for how many seconds player is outside of village collider
//updates from gamemanager number of enemies killed to speed up bar fill time
//spawns death 1 second after bar fills 
//waits 5 seconds after player enters trigger to reduce death meter and despawn death
//when meter empties bar speed is set to original

public class DeathSpawnManager : MonoBehaviour
{
    [SerializeField]GameObject deathPrefab;
    PlayerController playerController;
    GameObject player;

    float playerExploreTime;
    [SerializeField]float fillAmount;
    [SerializeField] float barFillIncrementOrig = .01f;
    [SerializeField] float spawnRange;

    public float barFillIncrement;
    float barFillSpeed =1;
    
    
    


    bool isPlayerExploring;
    bool isBarFillBusy;
    public bool isDeathActive;
    public bool isWaitingToDespawn;

    private bool isDeathSpawned = false; //Tracks if Death is spawned or not.
    //private GameObject deathInstance;

    // Start is called before the first frame update
    void Start()
    {
        

        barFillIncrement = barFillIncrementOrig;
        fillAmount = 0;
        player = GameManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {

        //checks the if the player is exploring, if the bar fill is currently in a coroutine, and if death is active
        if (isPlayerExploring && !isBarFillBusy && !isDeathActive) 
        {
            StartCoroutine(BarFill());
        }
        else if(!isPlayerExploring && !isBarFillBusy && !isDeathActive)
        {
            StartCoroutine(BarReduce());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        isPlayerExploring = false;
        if(isDeathActive == true)
        {
            StartCoroutine(WaitToDespawn());
        }
    }

    //if the player leaves the safe area sets the exploring bool to true 
    //if there is a counter going for deaths despawn, stops the counter and sets the bool to false
    private void OnTriggerExit(Collider other)
    {
        isPlayerExploring = true;
        if (isWaitingToDespawn)
        {
            StopCoroutine(WaitToDespawn());
            isWaitingToDespawn = false;
        }
    }

    public void IncreaseBarFillSpeed()
    {
        barFillIncrement = barFillIncrementOrig + (.01f * GameManager.instance.enemiesToSpawnDeath);
    }

    private void ResetBarFillSpeed()
    {
        if(barFillIncrement <= barFillIncrementOrig) barFillIncrement = barFillIncrementOrig;
        else if(barFillIncrement != barFillIncrementOrig)
        {
            barFillIncrement -= .01f;
        }
        if(GameManager.instance.enemiesToSpawnDeath > 0)
        {
            GameManager.instance.enemiesToSpawnDeath--;
        }
    }

    
    IEnumerator BarFill()
    {
        
        isBarFillBusy = true;   //prevents other enumerators from adjusting the bar fill while active

        if (fillAmount + barFillIncrement >= 1)
            fillAmount = 1;
        else
            fillAmount += barFillIncrement;//fills by increment

        if(fillAmount < 1)
            yield return new WaitForSeconds(barFillSpeed); //waits for bar fill speed amount
        
        if(fillAmount == 1)
        {
            yield return new WaitForSeconds(1f);
            SpawnDeath();
            TrackPlayerWithDeath();
        }


        isBarFillBusy = false;
    }


    //Bar reduces the fill amount 2x faster than it takes to fill it, calls to reduce enemy kill count and slow down
    //deaths awareness fill.
    IEnumerator BarReduce()
    {
        
        isBarFillBusy = true;//stops update from calling to fill until complete

        if(fillAmount - barFillIncrement <= 0) //checks to make sure fill amount doesn't go into the negative
        { fillAmount = 0; }   
        else fillAmount -= barFillIncrement; 


        yield return new WaitForSeconds(barFillSpeed); 
        if(barFillIncrement != barFillIncrementOrig || GameManager.instance.enemyCount > 0)
        {
            ResetBarFillSpeed(); //increments 
        }
        isBarFillBusy = false;  //tells update bar fill/reduce can be called again
    }


    //Starts a countdown to despawn death
    //this prevents the player from just running to safety to despawn death instantly
    IEnumerator WaitToDespawn()
    {
        isWaitingToDespawn = true;
        
            
            yield return new WaitForSeconds(3f);
            
            if (isPlayerExploring == true)
            { isWaitingToDespawn = false;}
            
            else if(isPlayerExploring == false)
            {
            //if the player does not leave the village in the time death despawns

            GameManager.instance.death.SetActive(false);
            isWaitingToDespawn = false;
                isDeathActive = false;
                
             }
    }
    private void SpawnDeath()
    {
        //Vector3 spawnPosition = player.transform.position + new Vector3(10, 5, 10);
        //deathInstance = Instantiate(deathPrefab, spawnPosition, Quaternion.identity);
        //isDeathActive = true;
        Vector3 spawnPos = GameManager.instance.player.transform.position + UnityEngine.Random.insideUnitSphere * spawnRange;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPos, out hit, spawnRange, 15))
        {
            spawnPos = hit.position;
        }
        
        GameManager.instance.death.transform.position = spawnPos;
        GameManager.instance.death.SetActive(true);
        isDeathActive = true;
        
        
    }

    private void TrackPlayerWithDeath()
    {
        if (GameManager.instance.death.activeInHierarchy)
        {
            
            GameManager.instance.death.GetComponent<deathController>().SetTarget(GameManager.instance.player.transform);
        }
    }

    public float GetCurrentFillAmount()
    {
        return fillAmount;
    }

}
