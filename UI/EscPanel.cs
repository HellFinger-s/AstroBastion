using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EscPanel : MonoBehaviour
{
    [SerializeField] private GameObject panel;

    public void EnablePanel()
    {
        if (Time.timeScale > 0)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            panel.SetActive(true);
            Time.timeScale = 0f;
            LUbus.GetInstance().LockControl();
        }

    }

    public void DisablePanel()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        panel.SetActive(false);
        Time.timeScale = 1f;
        LUbus.GetInstance().SwitchToShip();
        LUbus.GetInstance().FreeControl();
    }

    public void TogglePanel()
    {
        if (panel.activeSelf)
        {
            DisablePanel();
        }
        else
        {
            EnablePanel();
        }
    }

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
        Time.timeScale = 1f;
    }
}
