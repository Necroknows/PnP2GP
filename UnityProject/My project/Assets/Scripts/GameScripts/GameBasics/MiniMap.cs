using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Rendering;

public class MiniMap : MonoBehaviour
{
    [SerializeField] Transform mmCamPos;
    

    float yValue;
    private void Start()
    {
        yValue = mmCamPos.position.y;
        
    }
    void Update()
    {
        //camera will follow player
        // mmCamPos.TransformVector(GameManager.instance.player.transform.position.x, 0, GameManager.instance.player.transform.position.z);
        mmCamPos.position = new Vector3(GameManager.instance.player.transform.position.x, yValue, GameManager.instance.player.transform.position.z);
    }

    
}
