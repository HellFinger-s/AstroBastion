using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeDestroyTextTemplate : IButtonTextTemplate
{
    public string ReturnText(CMButton button)
    {
        Cube cube = (Cube) Builder.GetInstance().GetBuildableInfo(BuildableTypes.Cube).buildable;
        string text = "";
        Dictionary<string, string> substitutions = cube.GetSubstitutions(this);
        text = Localizer.GetInstance().Localize(LocalizationKeys.CubeDestroy.ToString());
        foreach (var kvp in substitutions)
        {
            text = text.Replace("{" + kvp.Key.ToString() + "}", kvp.Value);
        }
        return text;
    }
}
