using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PolicyTextTemplate : IButtonTextTemplate
{
    public string ReturnText(CMButton button)
    {
        BaseTower tower = Builder.GetInstance().selectedTower;
        return Localizer.GetInstance().Localize(tower.policy.ToString());
    }
}
