using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Wave", menuName = "ScriptableObjects/WavesPlanning/Wave", order = 1)]
public class Wave : ScriptableObject
{
    public float preparationSeconds;
    public List<Section> waveSections;
}
