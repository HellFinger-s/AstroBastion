using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AYellowpaper.SerializedCollections;
using UnityEngine.Serialization;

[System.Serializable]
public class InnerList<T>
{
    public List<T> innerList;
}


public class LUbus : LazySingleton<LUbus>
{
    public SO_Icons iconsDB;
    public BuildableDB buildableDB;

    public ResourcePanel resourcePanel;
    public WavePanel wavePanel;
    public PreparationPanel preparatonPanel;
    [FormerlySerializedAs("controllMatrix")]
    public ControlMatrix controlMatrix;
    public EnemyInfo enemyInfo;
    public WaveInfoShow waveInfoShow;
    public Bar healthBar;
    public Bar engineTempBar;
    public GameObject winPanel;
    public GameObject defeatPanel;
    public RespawnTimeout respawnTimeout;
    public EscPanel escPanel;
    public TowerUnderAttackIndicator towerAttackedIndicator;
    public ProtectableHP protectableHP;
    protected override bool CheckDependencies()
    {
        return true;
    }

    protected override void OnDependenciesReady()
    {
        return;
    }

    public void SwitchState(CMStatesKeys stateKey)
    {
        controlMatrix.SwitchState(stateKey);
    }

    public void SwitchSelectedToRight()
    {
        controlMatrix.SwitchSelectedToRight();
    }

    public void SwitchSelectedToLeft()
    {
        controlMatrix.SwitchSelectedToLeft();
    }

    public void PressSelectedButton()
    {
        controlMatrix.PressSelectedButton();
    }

    public void EnableRaycastBlocker()
    {
        controlMatrix.EnableRaycastBlocker();
    }

    public void DisableRaycastBlocker()
    {
        controlMatrix.DisableRaycastBlocker();
    }

    public void EndPreparation()
    {

    }

    public float[] GetPathsPassing()
    {
        return PoolManager.GetInstance().GetPathsPassing();
    }

    public void ShowEnemyInfo(BaseEnemy enemy)
    {
        enemyInfo.ShowInfo(enemy);
    }

    public void DisableEnemyInfo()
    {
        enemyInfo.DisableInfo();
    }

    public void UpdateResourcesValue(float value)
    {
        resourcePanel.UpdateValue(value);
    }

    public float GetResources()
    {
        return ResourceManager.GetInstance().GetResources();
    }

    public void UpdateWaveNumber(int waveNumber, int wavesCount)
    {
        wavePanel.UpdateWaveNumber(waveNumber, wavesCount);
    }

    public void StartPreparation(float seconds)
    {
        preparatonPanel.StartPreparation(seconds);
    }

    public void PreparationInterrupted()
    {
        preparatonPanel.PreparationInterrupted();
    }

    public void UpdateHealthBarValue(float value, float maxValue)
    {
        healthBar.UpdateValue(value, maxValue);
    }

    public void UpdateTempBarValue(float value, float maxValue)
    {
        engineTempBar.UpdateValue(value, maxValue);
    }

    public void ShowWaveInfo(string waveInfo, Vector2 newPosition)
    {
        waveInfoShow.ShowWaveInfo(waveInfo, newPosition);
    }

    public void EnableWinPanel()
    {
        winPanel.SetActive(true);
        Cursor.visible = true;
    }

    public void EnableDefeatPanel()
    {
        defeatPanel.SetActive(true);
        Cursor.visible = true;
    }

    public void StartRespawnCount(float value)
    {
        respawnTimeout.StartCount(value);
    }

    public void ToggleEscPanel()
    {
        escPanel.TogglePanel();
    }

    public void SwitchToShip()
    {
        PoolManager.GetInstance().player.SwitchToShip();
    }

    public void LockControl()
    {
        PoolManager.GetInstance().player.controlLocked = true;
    }

    public void FreeControl()
    {
        PoolManager.GetInstance().player.controlLocked = false;
    }

    public void TowerAttacked()
    {
        towerAttackedIndicator.Animate();
    }

    public void UpdateProtectableHP(int value, bool init=false)
    {
        protectableHP.UpdateHP(value, init);
    }
}
