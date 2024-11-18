using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPref : MonoBehaviour
{
    // Start is called before the first frame update
    public static PlayerPref instance;

    public float mouseSens;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        mouseSens = 100;
    }

    public void SetMouseSense(float amount)
    {
        mouseSens = amount;
    }
}
