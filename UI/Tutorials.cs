using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public enum TutorialKeys
{
    ShipControl,
    Line,
    LetsBuild,
    BuildMenu,
    AfterBuild,
    Geometry
}

public class Tutorials : LazySingleton<Tutorials>
{
    [SerializedDictionary]
    public SerializedDictionary<TutorialKeys, GameObject> tutorials = new SerializedDictionary<TutorialKeys, GameObject> { };

    private GameObject currentTutorial = null;
    private TutorialKeys currentKey;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (currentTutorial is not null)
            {
                currentTutorial.SetActive(false);
                Time.timeScale = 1f;
                PlayerPrefs.SetInt(currentKey.ToString(), 1);
                LUbus.GetInstance().FreeControl();
                currentTutorial = null;
            }
        }
    }


    public void ShowTutorial(TutorialKeys key)
    {
        if (!PlayerPrefs.HasKey(key.ToString()))
        {
            Time.timeScale = 0f;
            currentTutorial = tutorials[key];
            currentTutorial.SetActive(true);
            currentKey = key;
            LUbus.GetInstance().LockControl();
        }
    }

    protected override bool CheckDependencies()
    {
        return true;
    }

    protected override void OnDependenciesReady()
    {
        return;
    }
}
