using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : LazySingleton<ResourceManager>
{
    public int startResources = 100;

    private float resources;

    public bool IsEnoughResources(int cost)
    {
        return resources >= cost;
    }

    public void DecreaseResources(int value)
    {
        resources -= value;
        LUbus.GetInstance().UpdateResourcesValue(resources);
    }

    public void IncreaseResources(float value)
    {
        resources += value;
        LUbus.GetInstance().UpdateResourcesValue(resources);
    }

    protected override bool CheckDependencies()
    {
        return true;
    }

    protected override void OnDependenciesReady()
    {
        resources = startResources;
        return;
    }

    public float GetResources()
    {
        return resources;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H) && Input.GetKeyDown(KeyCode.M))
        {
            IncreaseResources(9999f);
        }
    }
}
