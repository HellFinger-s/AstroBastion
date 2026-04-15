using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileEffectsNames
{
    EngineOverheat,
    SlowDown,
    BackTeleport,
    GunSystemsReboot,
    Collapse,
    ShieldIgnore
}

public abstract class ProjectileEffect : MonoBehaviour
{
    public bool modifierEnabled = false;
    public int percent;
    public float value;
}
