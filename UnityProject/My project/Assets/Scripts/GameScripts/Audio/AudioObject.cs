using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayerMusic : MonoBehaviour
{
    public bool playerInArea;
    [SerializeField] AudioClip musicSource;
    [SerializeField] AudioListener playerCamera;
    [SerializeField] float maxDistance;
    [SerializeField] float minDistance;
    [SerializeField] float volumeMax0To1;

    AudioSource source;
    float distanceFactor;
    float realDistance;

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.clip = musicSource;
    }

    // Update is called once per frame
    void Update()
    {
        if (musicSource != null && playerInArea == true && source.isPlaying == false)
        {
            source.loop = true;
            source.maxDistance = maxDistance;
            source.Play();
        }
        UpdateVolume();
    }

    void UpdateVolume()
    {
        realDistance = Vector3.Distance(playerCamera.transform.position, this.transform.position);
        distanceFactor = 1 - ((realDistance) / (maxDistance));
        if (distanceFactor > 0 && realDistance > minDistance)
        {
            source.volume = distanceFactor * volumeMax0To1;
        }
        else if (realDistance <= minDistance)
        {
            source.volume = volumeMax0To1;
        }
        else
        {
            source.volume = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        playerInArea = true;
    }

}
