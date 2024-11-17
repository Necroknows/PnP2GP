using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // make the cursor visible to interact with menus
        Cursor.visible = true;
        // keep the cursor in the play area 
        Cursor.lockState = CursorLockMode.Confined;
    }

}
