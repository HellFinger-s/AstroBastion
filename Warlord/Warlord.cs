using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Warlord : MonoBehaviour
{
    public List<Wave> waves;

    private int waveIndex = 0;

    public List<EnemyPath> paths;

    public bool preparation = false;

    public AudioSource source;
    public AudioClip waveStartSound;

    private Coroutine preparationCoroutine;

    private IEnumerator StartWave()
    {
        source.Play();
        for (int sectionIndex = 0; sectionIndex < waves[waveIndex].waveSections.Count; sectionIndex++)
        {
            Section currentSection = waves[waveIndex].waveSections[sectionIndex];
            for (int enemyIndex = 0; enemyIndex < currentSection.count; enemyIndex++)
            {
                if (currentSection.useFormation)
                {
                    Formation formation = PoolManager.GetInstance().TakeFormation(currentSection.formationKey);
                    formation.transform.position = paths[currentSection.pathIndex].parentPortal.formationStartPlace.position;
                    for (int i = 0; i < formation.units.Count; i++)
                    {
                        BaseEnemy enemy = PoolManager.GetInstance().TakeEnemy(formation.units[i].enemyKey);
                        enemy.SetPath(paths[currentSection.pathIndex].spline,
                            formation.units[i].transform.position - formation.transform.position,
                            currentSection.pathIndex, true, formation.units[i].transform.position);
                        enemy.Setup(formation.units[i].setupParameters, waveIndex + 1);
                        enemy.Refresh();
                        enemy.gameObject.SetActive(true);
                    }
                }
                else
                {
                    //Debug.LogError(currentSection.enemyType);
                    BaseEnemy enemy = PoolManager.GetInstance().TakeEnemy(currentSection.enemyType);
                    PoolManager.GetInstance().AddEnemyToAlive(enemy);
                    enemy.SetPath(paths[currentSection.pathIndex].spline,
                        paths[currentSection.pathIndex].GetOffset(currentSection.spawnSpreadRadius),
                        currentSection.pathIndex, false, Vector3.zero);
                    enemy.Setup(waves[waveIndex].waveSections[sectionIndex].setupParameters, waveIndex + 1);
                    enemy.Refresh();
                    enemy.gameObject.SetActive(true);
                }

                if (enemyIndex == currentSection.count - 1)
                {
                    yield return new WaitForSeconds(currentSection.secondsAfterAllSpawn);
                }
                else
                {
                    yield return new WaitForSeconds(currentSection.secondsBetweenSpawn);
                }
            }
        }
        waveIndex++;
        if (waveIndex < waves.Count)
        {
            preparationCoroutine = StartCoroutine(Preparation());
        }
    }

    public void Activate()
    {
        LUbus.GetInstance().UpdateWaveNumber(waveIndex, waves.Count);
        preparationCoroutine = StartCoroutine(Preparation());
    }

    private IEnumerator Preparation()
    {
        preparation = true;
        LUbus.GetInstance().StartPreparation(waves[waveIndex].preparationSeconds);
        UpdateWaveInfoInPortals();
        yield return new WaitForSeconds(waves[waveIndex].preparationSeconds);
        StartCoroutine(StartWave());
        LUbus.GetInstance().UpdateWaveNumber(waveIndex + 1, waves.Count);
        LUbus.GetInstance().EndPreparation();
        preparation = false;
    }

    public void InterruptPreparation()
    {
        if (preparationCoroutine is not null)
        {
            StopCoroutine(preparationCoroutine);
        }
        LUbus.GetInstance().PreparationInterrupted();
        StartCoroutine(StartWave());
        LUbus.GetInstance().UpdateWaveNumber(waveIndex + 1, waves.Count);
        LUbus.GetInstance().EndPreparation();
        preparation = false;
    }

    private void UpdateWaveInfoInPortals()
    {
        for (int i = 0; i < paths.Count; i++)
        {
            paths[i].parentPortal.ClearInfo();
        }

        for (int sectionIndex = 0; sectionIndex < waves[waveIndex].waveSections.Count; sectionIndex++)
        {
            Section currentSection = waves[waveIndex].waveSections[sectionIndex];
            for (int enemyIndex = 0; enemyIndex < currentSection.count; enemyIndex++)
            {
                if (currentSection.useFormation)
                {
                    Formation formation = PoolManager.GetInstance().TakeFormation(currentSection.formationKey);
                    formation.transform.position = paths[currentSection.pathIndex].parentPortal.formationStartPlace.position;
                    for (int i = 0; i < formation.units.Count; i++)
                    {
                        paths[currentSection.pathIndex].parentPortal.AddWaveInfo(formation.units[i].enemyKey, 1);  
                    }
                }
                else
                {
                    paths[currentSection.pathIndex].parentPortal.AddWaveInfo(currentSection.enemyType, 1);
                }
            }
        }

        for (int i = 0; i < paths.Count; i++)
        {
            paths[i].parentPortal.ClearText();
            paths[i].parentPortal.CombineText();
        }
    }

    public bool IsAllWavesStarted()
    {
        return waveIndex == waves.Count;
    }
}
