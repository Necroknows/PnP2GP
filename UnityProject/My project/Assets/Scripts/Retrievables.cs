using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrievableObjects : MonoBehaviour
{
    //position by gamemanager
    //update position if dropped

    public bool isRetrieved { get; private set; } = false;

    //functions
    //reset retrieved status
    //reset object entirely

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //marks if retrieved or not
    public void SetRetrieved()
    {
        isRetrieved = true;
    }

    //used to reset state if needed
    public void Retrieve()
    {
        if(!isRetrieved)
        {
            //set object as retrieved
            isRetrieved = true;
            //deactive object
            gameObject.SetActive(false);
        }
    }

    //reset function to mark as not retrieved
    public void ResetObject()
    {
        isRetrieved = false;
        //reactivate object if deactivated
        gameObject.SetActive(false);
    }
}
