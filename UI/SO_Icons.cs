using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public enum IconsKeys
{
    Confirm,
    Reject,
    Star1,
    Star2,
    Star3,
    DestroyObject,
    Repair,
    QueuePolicy,
    NearestPolicy,
    FurtherPolicy,
    DyingPolicy,
    FreshestPolicy,
    FastestPolicy,
    SlowestPolicy,
    DestroyCube
}

public enum IconTypes
{
    Hidden,
    LeftUpgrade,
    RightUpgrade,
    Repair,
    Policy,
    DestroyObject,
    IncreaseLevel,
    Confirm,
    Reject,
    BuildableIcon,
    DestroyCube,
    LeftLevel4Tower,
    RightLevel4Tower
}

[CreateAssetMenu(fileName = "IconsDB", menuName = "ScriptableObjects/Icons_DB", order = 1)]
public class SO_Icons : ScriptableObject
{
    [SerializedDictionary]
    public SerializedDictionary<IconsKeys, Sprite> database;

    public Sprite GetPolicyIcon(TargetPolicy policy)
    {
        switch (policy)
        {
            case TargetPolicy.queue:
                return database[IconsKeys.QueuePolicy];

            case TargetPolicy.nearest:
                return database[IconsKeys.NearestPolicy];

            case TargetPolicy.further:
                return database[IconsKeys.FurtherPolicy];

            case TargetPolicy.dying:
                return database[IconsKeys.DyingPolicy];

            case TargetPolicy.freshest:
                return database[IconsKeys.FreshestPolicy];

            case TargetPolicy.fastest:
                return database[IconsKeys.FastestPolicy];

            case TargetPolicy.slowest:
                return database[IconsKeys.SlowestPolicy];

            default:
                Debug.LogWarning("Unknown policy");
                return database[IconsKeys.FurtherPolicy];
        }
    }

    public Sprite GetLevelIcon(int level)
    {
        if (level == 1)
        {
            return database[IconsKeys.Star1];
        }
        else if (level == 2)
        {
            return database[IconsKeys.Star2];
        }
        else if (level == 3)
        {
            return database[IconsKeys.Star3];
        }

        return database[IconsKeys.Star1];
    }

}
