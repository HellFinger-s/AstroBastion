using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(fileName = "EnemyDB", menuName = "ScriptableObjects/Enemy_DB", order = 2)]
public class EnemyDB : ScriptableObject
{
    [SerializedDictionary]
    public SerializedDictionary<EnemyTypes, BaseEnemy> database;
}
