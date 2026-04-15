using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneActivator : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Skybox={(RenderSettings.skybox ? RenderSettings.skybox.name : "NULL")}, Ambient={RenderSettings.ambientLight}, Fog={RenderSettings.fog}");
        if (scene.name == gameObject.scene.name)
            SceneManager.SetActiveScene(scene);
    }
}
