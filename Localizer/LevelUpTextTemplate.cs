using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpTextTemplate : IButtonTextTemplate
{
    public string ReturnText(CMButton button)
    {
        BaseTower tower = Builder.GetInstance().selectedTower;
        string text = "";
        Dictionary<string, string> substitutions = tower.GetSubstitutions(this);
        text = Localizer.GetInstance().Localize(LocalizationKeys.LevelUp.ToString());
        foreach (var kvp in substitutions)
        {
            text = text.Replace("{" + kvp.Key.ToString() + "}", kvp.Value);
        }
        return text;
    }
}
