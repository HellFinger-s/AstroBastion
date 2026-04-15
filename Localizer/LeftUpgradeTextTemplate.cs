using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftUpgradeTextTemplate : IButtonTextTemplate
{
    public string ReturnText(CMButton button)
    {
        BaseTower tower = Builder.GetInstance().selectedTower;
        string text = "";
        Dictionary<string, string> substitutions = tower.GetSubstitutions(this);
        text = Localizer.GetInstance().Localize(tower.GetUpgradeLocalizationKey(true));
        foreach (var kvp in substitutions)
        {
            text = text.Replace("{" + kvp.Key + "}", kvp.Value);
        }
        return text;
    }
}
