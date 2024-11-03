using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    public float fadeTime;

    AudioSource musicTrack01, musicTrack02;
    [SerializeField] AudioClip defaultMusic;
    [SerializeField] float volumeForAreaMusic0To1;

    bool isPlayingTrack1;
    Coroutine fade;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
        musicTrack01 = gameObject.AddComponent<AudioSource>();
        musicTrack02 = gameObject.AddComponent<AudioSource>();
        musicTrack01.loop = true;
        musicTrack02.loop = true;
        musicTrack01.volume = volumeForAreaMusic0To1;
        musicTrack02.volume = volumeForAreaMusic0To1;
        isPlayingTrack1 = true;

        AudioSwap(defaultMusic);
    }

    public void AudioSwap(AudioClip newClip)
    {
        if (fade != null)
        {
            StopCoroutine(fade);
        }
        fade = StartCoroutine(FadeTrack(newClip));
    }

    public void ReturnToDefault()
    {
        AudioSwap(defaultMusic);
    }

    IEnumerator FadeTrack(AudioClip newClip)
    {
        float timeElapsed = 0f;
        if (isPlayingTrack1)
        {
            musicTrack02.clip = newClip;
            musicTrack02.Play();

            while (timeElapsed < fadeTime)
            {
                musicTrack02.volume = Mathf.Lerp(0, volumeForAreaMusic0To1, timeElapsed / fadeTime);
                musicTrack01.volume = Mathf.Lerp(volumeForAreaMusic0To1, 0, timeElapsed / fadeTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            musicTrack01.Stop();
        }
        else
        {
            musicTrack01.clip = newClip;
            musicTrack01.Play();

            while (timeElapsed < fadeTime)
            {
                musicTrack01.volume = Mathf.Lerp(0, volumeForAreaMusic0To1, timeElapsed / fadeTime);
                musicTrack02.volume = Mathf.Lerp(volumeForAreaMusic0To1, 0, timeElapsed / fadeTime);
                timeElapsed += Time.deltaTime;
                yield return null;
            }

            musicTrack02.Stop();
        }

        isPlayingTrack1 = !isPlayingTrack1;
    }
}
