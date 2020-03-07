using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleFullScreen : MonoBehaviour
{

    private void Start()
    {
        Screen.fullScreen = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F11))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }  
    }
}
