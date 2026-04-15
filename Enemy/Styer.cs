using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Styer : MonoBehaviour
{
    public BaseEnemy currentEnemy;
    public float boostSpeedMultiplier = 2f;
    public float boostTime = 3f;
    public float restTime = 4f;

    private float timer = 0f;
    private bool boost = false;
    private float slowDownMultiplier;

    private void Start()
    {
        slowDownMultiplier = 1 / boostSpeedMultiplier;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (boost && timer >= boostTime)
        {
            boost = false;
            timer = 0f;
            currentEnemy.MultiplySpeedMultiplier(slowDownMultiplier);
        }
        if (!boost && timer >= restTime)
        {
            boost = true;
            timer = 0f;
            currentEnemy.MultiplySpeedMultiplier(boostSpeedMultiplier);
        }
    }
}
