using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour , IDamage
{
    //model for current object
    [SerializeField] Renderer model;
    //model for prefab to replace it
    [SerializeField] GameObject modelDestroyed;
    [SerializeField] int HP;

    //gets the position and rotation of an object
    Vector3 modelDestroyedPos;
    Quaternion modelDestroyedRot;

    //gets the color of the object for flash red function
    Color colorOrig;
    // Start is called before the first frame update
    void Start()
    {
        modelDestroyedPos = model.transform.position;
        modelDestroyedRot = model.transform.rotation;
        colorOrig = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void takeDamage(int amount)
    {
        HP -= amount;

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            
            Destroy(gameObject);
            Instantiate(modelDestroyed, modelDestroyedPos, modelDestroyedRot);

        }
    }

    IEnumerator flashColor()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
