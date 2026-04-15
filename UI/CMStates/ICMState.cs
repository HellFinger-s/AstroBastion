using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlMatrixButtonsIds
{
    B_00,
    B_01,
    B_02,
    B_03,
    B_10,
    B_11,
    B_12,
    B_13
}


public interface ICMState
{
    public void Enter();
    public void Exit();
}
