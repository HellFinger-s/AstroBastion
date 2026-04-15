using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sprinter : MonoBehaviour
{
    public BaseEnemy currentEnemy;
    public float boostSpeedMultiplier = 2f;
    public float boostTime = 3f;

    private float slowDownMultiplier;
    private float timer = 0f;

    private void Start()
    {
        currentEnemy.MultiplySpeedMultiplier(boostSpeedMultiplier);
        slowDownMultiplier = 1 / boostSpeedMultiplier;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > boostTime)
        {
            currentEnemy.MultiplySpeedMultiplier(slowDownMultiplier);
            this.enabled = false;
        }
    }
}
