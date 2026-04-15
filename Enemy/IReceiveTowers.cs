using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReceiveTowers
{
    public void AddTower(BaseTower tower);

    public void RemoveTower(BaseTower tower);
}
