using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShellTower : BaseTower, IHealer, IReceiveTowers
{
    [Header("Shell")]
    public GameObject booster;
    public float boosterTimeout;
    public int basicRepair;
    public int basicRepairTowersCount;
    public float basicRepairReloadTime;

    public List<BaseTower> detectedTowers = new List<BaseTower> { };
    private float timer = 0f;

    private void Update()
    {
        if (isActive)
        {
            timer += Time.deltaTime;
            if (timer > basicRepairReloadTime)
            {
                RepairTowers();
                timer = 0f;
            }
        }
    }

    public void ReloadBooster()
    {
        StartCoroutine(wait());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(boosterTimeout);
        booster.SetActive(true);
    }

    public void AddTower(BaseTower tower)
    {
        if (tower != this && !detectedTowers.Contains(tower))
        {
            InsertByDistance(tower);
        }
    }

    public void RemoveTower(BaseTower tower)
    {
        if (detectedTowers.Contains(tower))
        {
            detectedTowers.Remove(tower);
        }
    }

    public void InsertByDistance(BaseTower tower)
    {
        float towerDistSq = (tower.transform.position - transform.position).sqrMagnitude;

        int i = 0;
        for (; i < detectedTowers.Count;)
        {
            var t = detectedTowers[i];
            if (!t.gameObject.activeSelf)
            {
                detectedTowers.RemoveAt(i);
                continue;
            }

            float dSq = (t.transform.position - transform.position).sqrMagnitude;

            if (towerDistSq < dSq)
                break;
            i++;
        }

        detectedTowers.Insert(i, tower);
    }

    private void RepairTowers()
    {
        int calculatedRepairCount = CalculateRepairingTowers();
        int count = calculatedRepairCount < detectedTowers.Count ? calculatedRepairCount : detectedTowers.Count;
        for (int i = 0; i < count; i++)
        {
            detectedTowers[i].Repair(CalculateRepairValue());
        }
    }

    private int CalculateRepairValue()
    {
        return basicRepair + leftUpgradeLevel;
    }

    private int CalculateRepairingTowers()
    {
        return basicRepairTowersCount + rightUpgradeLevel;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleOnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleOnTriggerExit(other);
    }
    public override Dictionary<string, string> GetSubstitutions(IButtonTextTemplate template)
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string> { };

        if (template is BuildableTextTemplate)
        {
            substitutions.Add(TextMarks.RepairValue.ToString(), CalculateRepairValue().ToString());
            substitutions.Add(TextMarks.TowersRepairCount.ToString(), CalculateRepairingTowers().ToString());
            substitutions.Add(TextMarks.Timeout.ToString(), boosterTimeout.ToString());
            substitutions.Add(TextMarks.HP.ToString(), maxHealth.ToString());
        }

        return substitutions;
    }
}
