using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    public int sceneIndex;

    public void SimpleLoad()
    {
        if (sceneIndex == -1)
        {
            SceneManager.LoadScene(PoolManager.GetInstance().gameObject.scene.name);
        }
        else
        {
            SceneManager.LoadScene(sceneIndex);
        }


    }
}
