using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Builder : LazySingleton<Builder>
{
    public ResourceManager resourceManager;

    public BuildableDB buildableDB;

    public BaseTower selectedTower;
    public Platform selectedPlatform;
    public Cube selectedCube;

    private Buildable visualizedBuild;

    private BuildableTypes clickedBuildableKey;

    protected override bool CheckDependencies() => true;

    protected override void OnDependenciesReady()
    {
        Debug.Log("Builder initialized");
    }

    public void ClickedOnBuildable(BuildableTypes key)
    {
        clickedBuildableKey = key;
    }

    public BuildableTypes GetClickedBuildableKey()
    {
        return clickedBuildableKey;
    }

    public void SetSelectedPlatform(Platform newSelected)
    {
        if (selectedPlatform)
        {
            selectedPlatform.SetMaterial(MaterialKeys.normal);
        }
        selectedPlatform = newSelected;
        newSelected.SetMaterial(MaterialKeys.interactable);
        selectedCube = selectedPlatform.parentCube;
    }

    public void SetSelectedTowerAndPlatform(BaseTower newSelected)
    {
        selectedTower = newSelected;
        selectedCube = selectedTower.connectedPlatform.parentCube;
        selectedPlatform = selectedTower.connectedPlatform;
        selectedTower.SetAttackAreaVisibility(true);
    }


    public void ClearSelectedTower()
    {
        if (selectedTower)
        {
            selectedTower.SetMaterial(MaterialKeys.normal);
            selectedTower.SetAttackAreaVisibility(false);
        }
        selectedTower = null;
    }

    public void ClearSelectedPlatform()
    {
        if (selectedPlatform)
        {
            selectedPlatform.SetMaterial(MaterialKeys.normal);
        }
        selectedPlatform = null;
    }


    public void Build()
    {
        selectedPlatform.SetMaterial(MaterialKeys.normal);
        if (visualizedBuild is Cube cube)
        {
            //Debug.Log(selectedPlatform.cubeBuildPlace.position);
            cube.transform.position = selectedPlatform.cubeBuildPlace.position;
            cube.gameObject.SetActive(true);
            selectedPlatform = null;
            selectedTower = null;
            resourceManager.DecreaseResources(cube.cost);
        }
        else if (visualizedBuild is BaseTower baseTower)
        {
            if (selectedTower != baseTower)
            {
                selectedTower = null;
            }
            if (selectedTower != null)
            {
                selectedTower.Destroy();
            }
            baseTower.SetAttackAreaVisibility(true);
            baseTower.transform.position = selectedPlatform.towerBuildPlace.position;
            baseTower.connectedPlatform = selectedPlatform;
            baseTower.transform.parent = selectedPlatform.transform;
            baseTower.transform.localEulerAngles = Vector3.zero;
            // Set rotation
            resourceManager.DecreaseResources(baseTower.cost);
            selectedPlatform.connectedTower = baseTower;
            baseTower.gameObject.SetActive(true);
            baseTower.ActivateTower();
            baseTower.Built();
            selectedTower = baseTower;
            baseTower.isActive = true;
        }
        else
        {
            Debug.LogWarning("Unknown Build key");
        }
        Tutorials.GetInstance().ShowTutorial(TutorialKeys.AfterBuild);
        visualizedBuild = null;
    }

    public void Destroy()
    {
        resourceManager.IncreaseResources(selectedTower.GetPlayerDestroyCashback());
        selectedTower.PlayerDestroy();
    }

    public void DestroyCube()
    {
        selectedCube.PlayerDestroy();
    }

    public void VisualizeNewBuildable()
    {
        if (selectedTower is not null)
        {
            selectedTower.gameObject.SetActive(false);
        }
        visualizedBuild = PoolManager.GetInstance().TakeBuild(clickedBuildableKey);
        if (visualizedBuild is BaseTower baseTower)
        {
            baseTower.transform.position = selectedPlatform.towerBuildPlace.position;
            baseTower.connectedPlatform = selectedPlatform;
            baseTower.transform.parent = selectedPlatform.transform;
            baseTower.transform.localEulerAngles = Vector3.zero;
            baseTower.SetAttackAreaVisibility(true);
        }
        else if (visualizedBuild is Cube cube)
        {
            cube.transform.position = selectedPlatform.cubeBuildPlace.position;
        }
        visualizedBuild.gameObject.SetActive(true);
    }

    public void HideNewBuildable()
    {
        PoolManager.GetInstance().ReleaseBuild(visualizedBuild);
        visualizedBuild.gameObject.SetActive(false);
        if (selectedTower is not null)
        {
            selectedTower.gameObject.SetActive(true);
            selectedTower.isActive = true;
        }
        if (visualizedBuild is Cube)
        {
            selectedPlatform.connectedCube = null;
        }
        else if (visualizedBuild is BaseTower tower)
        {
            tower.SetAttackAreaVisibility(false);
        }
        if (selectedTower is not null)
        {
            selectedTower.SetAttackAreaVisibility(true);
        }
        visualizedBuild = null;
    }

    public void IncreaseTowerLevel()
    {
        BaseTower nextTower = (BaseTower) PoolManager.GetInstance().TakeBuild(selectedTower.nextTowerKey);
        nextTower.transform.position = selectedTower.connectedPlatform.towerBuildPlace.position;
        nextTower.connectedPlatform = selectedTower.connectedPlatform;
        nextTower.transform.parent = selectedTower.connectedPlatform.transform;
        nextTower.transform.localEulerAngles = Vector3.zero;
        nextTower.policy = selectedTower.policy;
        resourceManager.DecreaseResources(selectedTower.nextLevelCost);
        selectedTower.Destroy();
        selectedTower = nextTower;
        selectedPlatform.connectedTower = selectedTower;
        selectedTower.SetAttackAreaVisibility(true);
        selectedTower.gameObject.SetActive(true);
        selectedTower.isActive = true;
    }

    public int IncreaseTowerLeftUpgradeLevel()
    {
        if (resourceManager.IsEnoughResources(selectedTower.GetLeftUpgradeCost()))
        {
            resourceManager.DecreaseResources(selectedTower.GetLeftUpgradeCost());
            return selectedTower.IncreaseLeftUpgradeLevel();
        }
        return -1;
    }

    public int IncreaseTowerRightUpgradeLevel()
    {
        if (resourceManager.IsEnoughResources(selectedTower.GetRightUpgradeCost()))
        {
            resourceManager.DecreaseResources(selectedTower.GetRightUpgradeCost());
            return selectedTower.IncreaseRightUpgradeLevel();
        }
        return -1;
    }

    public void ChangeTowerPolicy(TargetPolicy newPolicy)
    {
        selectedTower.policy = newPolicy;
    }

    public BuildableRow GetBuildableInfo(BuildableTypes key) 
    {
        return buildableDB.database[key];
    }

    public BuildableDB GetBuildableDB()
    {
        return buildableDB;
    }

    public void RepairTower()
    {
        selectedTower.MomentalRepair();
    }

    public bool isFree()
    {
        return visualizedBuild == null;
    }

    public bool EnoughResourcesForBuild(BuildableTypes type)
    {
        return resourceManager.IsEnoughResources(buildableDB.database[type].buildable.cost);
    }
}
