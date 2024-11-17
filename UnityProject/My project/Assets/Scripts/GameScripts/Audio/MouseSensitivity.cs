using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivity : MonoBehaviour
{
    public float mouseSens;
    [SerializeField] Slider msSlider;

private void Start()
    {
        msSlider.value = .5f;
        
        
    }
    public void setSense()
    {
        if (msSlider.value == 0)
        {
            mouseSens = 100;
        }
        else
        {
            mouseSens = msSlider.value * 1000 ;

        }
        PlayerPref.instance.SetMouseSense(mouseSens);
    }
    public float getSense()
    {
        return mouseSens;
    }

}
