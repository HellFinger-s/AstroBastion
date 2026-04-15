using UnityEngine;
using AYellowpaper.SerializedCollections;

[System.Serializable]
public class BuildableRow
{
    public bool isBasic;
    public Buildable buildable;
}

[CreateAssetMenu(fileName = "BuildableDatabase", menuName = "ScriptableObjects/Buildable_DB", order = 1)]
public class BuildableDB : ScriptableObject
{
    [SerializedDictionary]
    public SerializedDictionary<BuildableTypes, BuildableRow> database;

    public Buildable GetBasic(int index)
    {
        int tempIndex = 0;
        foreach (BuildableTypes key in database.Keys)
        {
            if (database[key].isBasic)
            {
                if (tempIndex == index)
                    return database[key].buildable;
                tempIndex++;
            }
        }
        return null;
    }
}
