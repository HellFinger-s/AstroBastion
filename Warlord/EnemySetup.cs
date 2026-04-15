using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySetup
{
    public bool useShield = false;
    public int shieldCapacity;
    public float regenerationSpeed;
    public float regenerationCooldown;
    [Space]
    public bool overrideSpeed = false;
    public int newMaxSpeed;
}
