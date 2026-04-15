using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ButtonTextTemplates
{
    Buildable,
    CubeDestroy,
    LeftUpgrade,
    RightUpgrade,
    LevelUp,
    Policy,
    TowerDestroy,
    TowerRepair
}


public interface IButtonTextTemplate
{
    public string ReturnText(CMButton button);
}
