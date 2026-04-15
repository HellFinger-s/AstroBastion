using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bar : MonoBehaviour
{
    public Image bar;
    public float maxBarValue = 0.25f;
    public float changingSpeed = 10f;
    private float targetValue;
    public bool startFromZero = false;

    private float currentValue;
    private bool change = false;

    private void Awake()
    {
        if (!startFromZero)
        {
            currentValue = maxBarValue;
            targetValue = maxBarValue; ;
        }
        else
        {
            currentValue = 0f;
            targetValue = 0f;
        }
        bar.fillAmount = currentValue;
    }

    void Update()
    {
        if (change)
        {
            if (Mathf.Abs(currentValue - targetValue) > 1e-3)
            {
                currentValue = Mathf.Lerp(currentValue, targetValue, changingSpeed * Time.deltaTime);
            }
            else
            {
                change = false;
                currentValue = targetValue;
            }
            bar.fillAmount = currentValue;
        }
    }

    public void UpdateValue(float newValue, float maxValue)
    {
        targetValue = newValue * maxBarValue / maxValue;
        if (targetValue > maxBarValue)
        {
            targetValue = maxBarValue;
        }
        change = true;
    }
}
