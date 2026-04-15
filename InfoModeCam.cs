using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoModeCam : MonoBehaviour
{
    public Camera cam;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleCam();
        }
    }

    private void ToggleCam()
    {
        cam.enabled = !cam.enabled;
    }
}
