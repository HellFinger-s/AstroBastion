using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Warlord warlord;


    private bool poolManagerLoaded = false;
    private bool interfaceLoaded = false;

    void Awake()
    {
        AsyncOperation loadUI = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
        loadUI.completed += InterfaceLoaded;
        PoolManager.OnInitialized += OnPoolManagerReady;
    }

    private void OnPoolManagerReady()
    {
        PoolManager.GetInstance().InitPools();
        PoolManager.OnInitialized -= OnPoolManagerReady;
        poolManagerLoaded = true;
        CheckUp();
    }

    private void InterfaceLoaded(AsyncOperation obj)
    {
        interfaceLoaded = true;
        CheckUp();
    }

    private void CheckUp()
    {
        if (interfaceLoaded && poolManagerLoaded)
        {
            warlord.Activate();
        }
    }

    public void ProtectableDead()
    {
        LUbus.GetInstance().EnableDefeatPanel();
    }

    public void WavesCleared()
    {
        LUbus.GetInstance().EnableWinPanel();
    }
}
