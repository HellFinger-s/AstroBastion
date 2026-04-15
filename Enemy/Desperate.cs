using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Desperate : MonoBehaviour
{
    public BaseEnemy currentEnemy;
    [Range(0, 1)]
    public float hpThreshold = 0.3f;
    public float boostSpeedMultiplier = 2f;

    private float updateInterval = 0.3f;
    private float timer = 0f;

    private void Tick()
    {
        timer += Time.deltaTime;
        if (timer > updateInterval)
        {
            timer = 0f;
            if (currentEnemy.GetCurrentHealth() / currentEnemy.GetScaledMaxHealth() < hpThreshold)
            {
                currentEnemy.MultiplySpeedMultiplier(boostSpeedMultiplier);
                this.enabled = false;
            }
        }
    }
}
