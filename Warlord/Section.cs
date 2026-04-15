using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SO_Sec", menuName = "ScriptableObjects/WavesPlanning/Section", order = 1)]
public class Section : ScriptableObject
{
    [Header("Simple heap")]
    public EnemyTypes enemyType;
    public EnemySetup setupParameters;

    [Header("Formation")]
    public bool useFormation;
    public FormationKeys formationKey;

    [Header("General settings")]
    public int count;
    public float secondsBetweenSpawn;
    public float secondsAfterAllSpawn;
    public int pathIndex;
    [Range(0, 100)]
    public int spawnSpreadRadius;
}
