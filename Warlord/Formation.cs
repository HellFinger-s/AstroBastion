using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public enum FormationKeys
{
    OxAndFly,
    FlySoldierSprinter,
    ProvocatorAndSoliders,
    RepairTeamAndSoldiers,
    RepairTeamAndShieldedSoldiers
}

public class Formation : MonoBehaviour
{
    public List<FormationUnit> units;

    [Button]
    public void CollectUnits()
    {
        units.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            units.Add(transform.GetChild(i).GetComponent<FormationUnit>());
        }
    }
}
