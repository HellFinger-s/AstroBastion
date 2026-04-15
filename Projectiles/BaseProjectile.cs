using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class BaseProjectile : MonoBehaviour
{
    [HideInInspector]
    public int poolIndex;
    [HideInInspector]
    public IReleaseProjectile parentPool;

    [SerializedDictionary]
    public SerializedDictionary<ProjectileEffectsNames, ProjectileEffect> effects;

    public virtual void Setup(float additiveSpeed) {}

    public virtual float GetSpeed() { return 100; }
}
