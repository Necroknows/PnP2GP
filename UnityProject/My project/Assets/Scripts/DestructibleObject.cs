using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour , IDamage
{
    //model for current object
    [SerializeField] Renderer model;
    //model for prefab to replace it
    [SerializeField] GameObject modelSwitch;
    [SerializeField] bool isUsingNewModel;
    [SerializeField] int HP;

    //gets the position and rotation of an object
    Vector3 modelSwitchPos;
    Quaternion modelSwitchRot;

    //gets the color of the object for flash red function
    Color colorOrig;
    // Start is called before the first frame update
    void Start()
    {
        modelSwitchPos = model.transform.position;
        modelSwitchRot = model.transform.rotation;
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(int amount,Vector3 Dir)
    {
        HP -= amount;

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            //destroys the model
            Destroy(gameObject);
            //if the bool isUsingNewModel is true and there is a prefab
            //attached for modelSwitch, will replace model to modelSwitch
            if (isUsingNewModel)
            {
                Instantiate(modelSwitch, modelSwitchPos, modelSwitchRot);
            }
        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
