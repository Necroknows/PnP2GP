using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
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
    float barFillSpeed;
    
    
    int barFillSpeedOrig = 1;


    bool isPlayerExploring;
    bool isBarFillBusy;
    bool isDeathActive;
    bool isWaitingToDespawn;

    private bool isDeathSpawned = false; //Tracks if Death is spawned or not.
    private GameObject deathInstance;

    // Start is called before the first frame update
    void Start()
    {
        barFillSpeed = barFillSpeedOrig;
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
        barFillSpeed = barFillSpeedOrig - (.1f * GameManager.instance.enemyCount);
    }

    private void ResetBarFillSpeed()
    {
        if(barFillSpeed > barFillSpeedOrig) barFillSpeed = barFillSpeedOrig;
        else if(barFillSpeed != barFillSpeedOrig)
        {
            barFillSpeed += .1f;
        }
        if(GameManager.instance.enemyCount > 0)
        {
            GameManager.instance.enemyCount--;
        }
    }

    
    IEnumerator BarFill()
    {
        
        isBarFillBusy = true;   //prevents other enumerators from adjusting the bar fill while active

        if (fillAmount + (float).01 >= 1)
            fillAmount = 1;
        else
            fillAmount += (float).01;//fills by increment

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

        if(fillAmount - .02 <= 0) //checks to make sure fill amount doesn't go into the negative
        { fillAmount = 0; }     
        else fillAmount -= (float).02; //otherwize it reduces by .02


        yield return new WaitForSeconds(barFillSpeed); 
        if(barFillSpeed != barFillSpeedOrig || GameManager.instance.enemyCount > 0)
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
        for(int i = 0; i < 5; i++)
        {
                yield return new WaitForSeconds(.5f);
            
            if(i == 5)
            {
                //if the player does not leave the village in the time death despawns
                deathInstance.gameObject.SetActive(false);
                isWaitingToDespawn = false;
                isDeathActive = false;
                BarReduce(); //begins to reduce awareness immediately after
            }
        }
    }
    private void SpawnDeath()
    {
        Vector3 spawnPosition = player.transform.position + new Vector3(10, 5, 10);
        deathInstance = Instantiate(deathPrefab, spawnPosition, Quaternion.identity);
        isDeathActive = true;
    }

    private void TrackPlayerWithDeath()
    {
        if (deathInstance != null)
        {
            deathController deathScript = deathInstance.GetComponent<deathController>();
            deathScript.SetTarget(player.transform);
        }
    }

    public float GetCurrentFillAmount()
    {
        return fillAmount;
    }

}
