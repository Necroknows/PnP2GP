using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FacePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    private Transform playerTransform;
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerTransform != null)
        {
            Vector3 direction = (playerTransform.position-transform.position).normalized;
            Quaternion lookRotation= Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation,lookRotation,Time.deltaTime*5f);
        }
    }

}
