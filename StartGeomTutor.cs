using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGeomTutor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Tutorials.GetInstance().ShowTutorial(TutorialKeys.Geometry);
    }
}
