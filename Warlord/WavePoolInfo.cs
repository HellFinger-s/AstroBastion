using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_WavePool", menuName = "ScriptableObjects/WavesPlanning/WavePoolInfo", order = 1)]
public class WavePoolInfo : ScriptableObject
{
    public EnemyTypes enemyType;

    public bool useFormation;
    public FormationKeys formationKey;
    
    public int count;
}
