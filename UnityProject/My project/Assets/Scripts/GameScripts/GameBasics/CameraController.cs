using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] int sens;
    [SerializeField] int lockVertMin, lockVertMax;
    [SerializeField] bool invertY;

    float rotX;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //get input
        float mouseY = Input.GetAxis("Mouse Y") * sens * Time.deltaTime;
        float mouseX = Input.GetAxis("Mouse X") * sens * Time.deltaTime;
        //rotation of the camera
        if (invertY)
        {
            rotX += mouseY;
        }
        else
        {
            rotX -= mouseY;
        }

        //clamp the rotX on the x-axis to the min and max
        rotX = Mathf.Clamp(rotX, lockVertMin, lockVertMax);

        //rotate the cam on the x-axis
        //using Quaternion library because we are dealing with angles
        transform.localRotation = Quaternion.Euler(rotX, 0, 0);

        //rotate the PLAYER on the y-axis
        //our camera is a child of the player, so we can transform via the parent
        transform.parent.Rotate(Vector3.up * mouseX);
    }
}
