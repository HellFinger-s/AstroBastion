using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    kinetic,
    energy
}



public class Damage : MonoBehaviour
{
    public int value;
    public int multiplier = 1;
    public DamageType damageType;
}
