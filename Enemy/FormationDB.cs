using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(fileName = "FormationDB", menuName = "ScriptableObjects/Formation_DB", order = 3)]
public class FormationDB : ScriptableObject
{
    [SerializedDictionary]
    public SerializedDictionary<FormationKeys, Formation> database;
}
