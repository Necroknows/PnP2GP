using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSwapper : MonoBehaviour
{
    [SerializeField] AudioClip newTrack;
    [SerializeField] float fadeTimer;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.fadeTime = fadeTimer;
            AudioManager.instance.AudioSwap(newTrack);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            AudioManager.instance.fadeTime = fadeTimer;
            AudioManager.instance.ReturnToDefault();
        }
    }
}
