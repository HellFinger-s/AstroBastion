using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildableTextTemplate : IButtonTextTemplate
{

    public string ReturnText(CMButton button)
    {
        Buildable build = Builder.GetInstance().GetBuildableInfo(button.attachedBuildableKey).buildable;
        string text = "";
        Dictionary<string, string> substitutions;
        if (build is Cube cube)
        {
            substitutions = cube.GetSubstitutions(this);
            text = Localizer.GetInstance().Localize(cube.descKey.ToString());
        }
        else if (build is BaseTower baseTower)
        {
            substitutions = baseTower.GetSubstitutions(this);
            text = Localizer.GetInstance().Localize(baseTower.descKey.ToString());
        }
        else
        {
            Debug.LogWarning("Unknown buildable");
            return "";
        }
        foreach (var kvp in substitutions)
        {
            text = text.Replace("{" + kvp.Key.ToString() + "}", kvp.Value);
        }
        return text;
    }
}
