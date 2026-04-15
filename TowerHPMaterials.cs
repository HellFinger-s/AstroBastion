using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerHpMats", menuName = "Data/Tower HP Set")]
public class TowerHPMaterials : ScriptableObject
{
    public Material maxHP;
    public Material midHP;
    public Material lowHP;
}
