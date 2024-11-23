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
        msSlider.value = PlayerPref.instance.mouseSens /1000;
        setSense();
        
    }
    public void setSense()
    {
        
            mouseSens = msSlider.value * 1000 ;

        
        PlayerPref.instance.SetMouseSense(mouseSens);
    }
    public float getSense()
    {
        return mouseSens;
    }

}
