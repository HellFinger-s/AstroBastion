using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseState : ScriptableObject, ICMState
{
    public List<InnerList<IconTypes>> controlMatrix;
    public int maxSelectedIndex = 3;

    public void Enter()
    {

    }

    public void Exit()
    {

    }
}
