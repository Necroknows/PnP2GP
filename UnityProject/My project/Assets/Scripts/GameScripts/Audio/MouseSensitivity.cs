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
        msSlider.value = PlayerPref.instance.mouseSens/200;
        
    }
    public void setSense()
    {
        
            mouseSens = msSlider.value * 200 ;

        
        PlayerPref.instance.SetMouseSense(mouseSens);
    }
    public float getSense()
    {
        return mouseSens;
    }

}
