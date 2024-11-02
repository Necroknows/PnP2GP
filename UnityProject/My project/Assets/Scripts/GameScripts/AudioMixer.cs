using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioMixer : MonoBehaviour
{
    
    [SerializeField] Slider slider;


    private void Start()
    {
        slider.value = AudioListener.volume;
    }

    public void setVolume()
    {
        float volume = slider.value;
        AudioListener.volume = volume;
    }
}

