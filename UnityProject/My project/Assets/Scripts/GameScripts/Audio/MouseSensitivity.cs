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
        setSense();
        
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
