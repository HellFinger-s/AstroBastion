using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StartShipTutor : MonoBehaviour
{
    public PlayerControl playerControl;

    private Vector3 pos;
    private bool lineCalled = false;

    private void Start()
    {
        Tutorials.GetInstance().ShowTutorial(TutorialKeys.ShipControl);
        pos = playerControl.transform.position;
    }


    private void Update()
    {
        if (!lineCalled && (playerControl.transform.position - pos).sqrMagnitude > 1e4)
        {
            Tutorials.GetInstance().ShowTutorial(TutorialKeys.Line);
            lineCalled = true;
            pos = playerControl.transform.position;
        }

        if (lineCalled && (playerControl.transform.position - pos).sqrMagnitude > 5e4)
        {
            Tutorials.GetInstance().ShowTutorial(TutorialKeys.LetsBuild);
            this.enabled = false;
        }
    }
}
