using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deathController : MonoBehaviour
{
    private Transform target;
    public float speed = 2.5f;

    public void SetTarget(Transform playerTransform)
    {
        target = playerTransform;
    }
   
    void Update()
    {
        if(target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;
        }
    }
}
