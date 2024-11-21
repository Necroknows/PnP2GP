using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Can be used on each enemy or new enemies to display HP Bar to player.
public class FacePlayerCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 180, 0); //Flips Health Bar to face camera.
    }
}
