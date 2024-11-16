using UnityEngine;

public class AudioRangeController : MonoBehaviour
{
    public string playerTag = "Player";
    public GameObject player;
    public float activationRange = 10f;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if(audioSource == null)
        {
            Debug.LogError("No AudioSource Component Founnd on " + gameObject.name);
        }

        //find player dynamically
        player = GameObject.FindGameObjectWithTag(playerTag);
        if(player == null)
        {
            Debug.LogError("No GameObject w/ Tag 'Player' Found In Scene");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(audioSource != null && player != null)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if(distance <= activationRange)
            {
                if(!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
            }
            else
            {
                if(audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }
}
