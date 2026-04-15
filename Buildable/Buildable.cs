using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BuildableTypes
{
    Cube,
    AutoTower_L1,
    AutoTower_L2,
    AutoTower_L3,
    LaserTower,
    RailgunTower,
    RocketTower_L1,
    RocketTower_L2,
    RocketTower_L3,
    NuclearTower,
    PlasmaTower,
    PitstopTower_L1,
    PitstopTower_L2,
    PitstopTower_L3,
    PortalTower,
    ShellTower,
    EnergyTower_L1,
    EnergyTower_L2,
    EnergyTower_L3,
    ElectronicTower,
    GravityTower
}

public class Buildable : MonoBehaviour
{
    [Header("Buildable settings")]
    public int cost = 100;
    public int fullCost;
    [Range(0, 1)]
    public float cashbackPercent = 0.8f;
    public Sprite selfIcon;
    public BuildableTypes keyName;
    public int poolIndex = 0;
}
