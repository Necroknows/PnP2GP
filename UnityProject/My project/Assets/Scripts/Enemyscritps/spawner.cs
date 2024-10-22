using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour, IDamage
{
    //rigid body for enemy prefab
    [SerializeField] GameObject enemy;
    //spawn point transform
    [SerializeField] Transform spawnPoint;
    //model for the spawner, used to flash red and be destroyed
    [SerializeField] Renderer model;
    //model for prefab to replace it
    [SerializeField] GameObject modelSwitch;
    [SerializeField] bool isUsingNewModel;


    //how many enemies to be spawned each wave
    [SerializeField] int spawnCount;
    //how often enemies will be spawned
    [SerializeField] int spawnRate;
    //health of the spawner(only used if continously spawning for the moment)
    [SerializeField] int HP;
    [SerializeField] bool indestructible;


    //gets the position and rotation of an object
    Vector3 modelSwitchPos;
    Quaternion modelSwitchRot;

    //color so it will flash red
    Color colorOrig;
    //for trigger enter/exit of player
    bool playerInRange;
    //if currently spawning
    bool isSpawning;
    
    //counts how many spawns there have been
    int spawnCounter;

    //continous waves of enemies spawned 
    [SerializeField] bool continousSpawning;

    // Start is called before the first frame update
    void Start()
    {
        if(isUsingNewModel)
        {
            modelSwitchPos = model.transform.position;
            modelSwitchRot = model.transform.rotation;
        }
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange)
        {
            //will continously spawn enemies until spawner is destroyed
            if(continousSpawning && !isSpawning)
            {
               
                    StartCoroutine(spawn());
                
            }
            //will only spawn enemies once
            if(!continousSpawning && !isSpawning &&spawnCounter <= 0)
            {
               
                
                    StartCoroutine(spawn());
                
            }
        }
    }
    public void takeDamage(int amount, Vector3 Dir)
    {
        if (!indestructible || this.GetComponent<DestructibleObject>() != null)
        {
            HP -= amount;

            StartCoroutine(flashRed());

            if (HP <= 0)
            {
                Destroy(gameObject);
                if (isUsingNewModel)
                {
                    Instantiate(modelSwitch, modelSwitchPos, modelSwitchRot);
                }
            }
        }
    }

    
    IEnumerator flashRed()
        //flashes spawner red when hit
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = colorOrig;
    }

    

    IEnumerator spawn()
        //spawns enemies
    {
        isSpawning = true;

        
        
        for (int numOfSpawn = 0; numOfSpawn < spawnCount; numOfSpawn++)
        {
            Instantiate(enemy, spawnPoint.position, transform.rotation);
        }
        yield return new WaitForSeconds(spawnRate);
        spawnCounter++;
        
        
        isSpawning = false;
    }



    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
