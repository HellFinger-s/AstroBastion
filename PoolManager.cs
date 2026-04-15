using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;



public class PoolManager : LazySingleton<PoolManager>
{
    private class PoolRow<T>
    {
        public int freeElementIndex = 0;
        public List<T> pool = new List<T> { };
    }

    public PlayerControl player;

    public BuildableDB buildableDB;
    public EnemyDB enemyDB;
    public FormationDB formationDB;

    public Warlord warlord;
    public LevelManager levelManager;

    public PortalTowersDispatcher dispatcher;

    public List<WavePoolInfo> enemyPoolInfo;

    private Dictionary<BuildableTypes, PoolRow<Buildable>> buildablePools = new Dictionary<BuildableTypes, PoolRow<Buildable>> { };
    private Dictionary<EnemyTypes, PoolRow<BaseEnemy>> enemyPools = new Dictionary<EnemyTypes, PoolRow<BaseEnemy>> { };
    private Dictionary<FormationKeys, PoolRow<Formation>> formationPools = new Dictionary<FormationKeys, PoolRow<Formation>> { };


    private List<BaseEnemy> aliveEnemy = new List<BaseEnemy> { };

    public int buildablePoolInitCapacity = 10;

    private int pathsCount;
    public float[] pathsPassingPercent = new float[5] { -1f, -1f, -1f, -1f, -1f};

    protected override bool CheckDependencies()
    {
        //Debug.Log(Builder.GetInstance() != null);
        pathsCount = warlord.paths.Count;
        for (int i = 0; i < pathsCount; i++)
        {
            pathsPassingPercent[i] = 0.01f;
        }
        return Builder.GetInstance() != null;
    }

    protected override void OnDependenciesReady()
    {
        Debug.Log("PoolManager initialized");
    }

    public void InitPools()
    {
        foreach (BuildableTypes key in Enum.GetValues(typeof(BuildableTypes)))
        {
            buildablePools.Add(key, new PoolRow<Buildable>());
            for (int i = 0; i < buildablePoolInitCapacity; i++)
            {
                if (Builder.GetInstance().GetBuildableInfo(key).buildable != null)
                {
                    buildablePools[key].pool.Add(GameObject.Instantiate(Builder.GetInstance().GetBuildableInfo(key).buildable, Vector3.zero, Quaternion.identity));
                    buildablePools[key].pool[i].poolIndex = i;
                    if (buildablePools[key].pool[i] is BaseTower baseTower)
                    {
                        baseTower.InitTower(warlord);
                        if (baseTower is PortalTower portalTower) // too bad
                        {
                            portalTower.dispatcher = dispatcher;
                        }
                    }
                    buildablePools[key].pool[i].gameObject.SetActive(false);
                }
                else
                    Debug.LogWarning("Skip  pool for   " + key);
            }
        }

        foreach (WavePoolInfo wavePoolInfo in enemyPoolInfo)
        {
            if (wavePoolInfo.useFormation)
            {
                formationPools.Add(wavePoolInfo.formationKey, new PoolRow<Formation>());
                for (int i = 0; i < wavePoolInfo.count; i++)
                {
                    formationPools[wavePoolInfo.formationKey].pool.Add(GameObject.Instantiate(formationDB.database[wavePoolInfo.formationKey], Vector3.zero, Quaternion.identity));
                    formationPools[wavePoolInfo.formationKey].pool[i].gameObject.SetActive(false);
                    //Debug.LogError("Pool formation   " + wavePoolInfo.formationKey);
                }
            }
            else
            {
                enemyPools.Add(wavePoolInfo.enemyType, new PoolRow<BaseEnemy>());
                //Debug.LogError(wavePoolInfo.enemyType);
                for (int i = 0; i < wavePoolInfo.count; i++)
                {
                    enemyPools[wavePoolInfo.enemyType].pool.Add(GameObject.Instantiate(enemyDB.database[wavePoolInfo.enemyType], Vector3.zero, Quaternion.identity));
                    enemyPools[wavePoolInfo.enemyType].pool[i].poolIndex = i;
                    //enemyPools[wavePoolInfo.enemyType].pool[i].currentHealth = enemyPools[wavePoolInfo.enemyType].pool[i].maxHealth;
                    enemyPools[wavePoolInfo.enemyType].pool[i].gameObject.SetActive(false);
                }
            }

        }
    }

    public Buildable TakeBuild(BuildableTypes key)
    {
        if (buildablePools[key].freeElementIndex < buildablePools[key].pool.Count)
        {
            if (buildablePools[key].freeElementIndex == buildablePools[key].pool.Count - 2)
            {
                ExpandTowerPool(key);
            }
            return buildablePools[key].pool[buildablePools[key].freeElementIndex++];
        }
        else
        {
            Debug.LogWarning("Towers pool overheat");
            return buildablePools[key].pool[0];
        }
    }

    public void ReleaseBuild(Buildable buildToRelease)
    {
        Buildable tempBuild = buildablePools[buildToRelease.keyName].pool[--buildablePools[buildToRelease.keyName].freeElementIndex];
        tempBuild.poolIndex = buildToRelease.poolIndex;
        buildablePools[buildToRelease.keyName].pool[tempBuild.poolIndex] = tempBuild;
        buildablePools[buildToRelease.keyName].pool[buildablePools[buildToRelease.keyName].freeElementIndex] = buildToRelease;
        buildToRelease.poolIndex = buildablePools[buildToRelease.keyName].freeElementIndex;
        buildToRelease.transform.position = Vector3.zero;
        buildToRelease.transform.parent = null;
        buildToRelease.gameObject.SetActive(false);
    }

    public BaseEnemy TakeEnemy(EnemyTypes key)
    {
        if (enemyPools[key].freeElementIndex < enemyPools[key].pool.Count)
        {
            if (enemyPools[key].freeElementIndex == enemyPools[key].pool.Count - 2)
            {
                ExpandEnemyPool(key);
            }
            return enemyPools[key].pool[enemyPools[key].freeElementIndex++];
        }
        else
        {
            Debug.LogWarning("Enemy pool overheat");
            return enemyPools[key].pool[0];
        }
    }

    public Formation TakeFormation(FormationKeys key)
    {
        return formationPools[key].pool[0];
    }

    public void ReleaseEnemy(BaseEnemy enemyToRelease)
    {
        aliveEnemy.Remove(enemyToRelease);
        BaseEnemy tempEnemy = enemyPools[enemyToRelease.keyName].pool[--enemyPools[enemyToRelease.keyName].freeElementIndex];
        tempEnemy.poolIndex = enemyToRelease.poolIndex;
        enemyPools[enemyToRelease.keyName].pool[tempEnemy.poolIndex] = tempEnemy;
        enemyPools[enemyToRelease.keyName].pool[enemyPools[enemyToRelease.keyName].freeElementIndex] = enemyToRelease;
        enemyToRelease.poolIndex = enemyPools[enemyToRelease.keyName].freeElementIndex;
        enemyToRelease.transform.position = Vector3.zero;
        if (aliveEnemy.Count == 0 && warlord.IsAllWavesStarted())
        {
            levelManager.WavesCleared();
        }
    }


    private void ExpandTowerPool(BuildableTypes key)
    {
        int newCount = buildablePools[key].pool.Count * 2;
        for (int i = buildablePools[key].pool.Count; i < newCount; i++)
        {
            buildablePools[key].pool.Add(GameObject.Instantiate(Builder.GetInstance().GetBuildableInfo(key).buildable, Vector3.zero, Quaternion.identity));
            buildablePools[key].pool[i].poolIndex = i;
            if (buildablePools[key].pool[i] is BaseTower baseTower)
            {
                baseTower.InitTower(warlord);
                if (baseTower is PortalTower portalTower) // too bad
                {
                    portalTower.dispatcher = dispatcher;
                }
            }
            buildablePools[key].pool[i].gameObject.SetActive(false);
        }
    }

    private void ExpandEnemyPool(EnemyTypes key)
    {
        int newCount = enemyPools[key].pool.Count * 2;
        for (int i = enemyPools[key].pool.Count; i < newCount; i++)
        {
            enemyPools[key].pool.Add(GameObject.Instantiate(enemyDB.database[key], Vector3.zero, Quaternion.identity));
            enemyPools[key].pool[i].poolIndex = i;
            //enemyPools[key].pool[i].currentHealth = enemyPools[key].pool[i].maxHealth;
            enemyPools[key].pool[i].gameObject.SetActive(false);
        }
    }

    public void AddEnemyToAlive(BaseEnemy enemy)
    {
        aliveEnemy.Add(enemy);
    }

    public float[] GetPathsPassing()
    {
        for (int i = 0; i < pathsCount; i++)
        {
            pathsPassingPercent[i] = 0;
        }

        for (int i = 0; i < aliveEnemy.Count; i++)
        {
            if (pathsPassingPercent[aliveEnemy[i].pathIndex] < aliveEnemy[i].GetPathPassing())
            {
                pathsPassingPercent[aliveEnemy[i].pathIndex] = aliveEnemy[i].GetPathPassing();
            }
        }
        return pathsPassingPercent;
    }
}
