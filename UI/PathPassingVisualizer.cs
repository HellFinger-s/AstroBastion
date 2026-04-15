using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathPassingVisualizer : MonoBehaviour
{
    public Slider alarmSlider;
    public Image alarmBulb;
    public Image pathBar;
    public float blinkSpeed = 2f;

    private bool isBlinking = false;
    private float time;

    private void Update()
    {
        if (!isBlinking)
            return;

        time += Time.deltaTime;
        float alpha = Mathf.PingPong(time * blinkSpeed, 1f);

        Color c = alarmBulb.color;
        c.a = alpha;
        alarmBulb.color = c;
    }

    public void UpdateInformation(float newPassing)
    {
        pathBar.fillAmount = newPassing;
        CheckAlarm();
    }

    public void EnablePath()
    {
        gameObject.SetActive(true);
    }

    public void DisablePath()
    {
        gameObject.SetActive(false);
    }

    private void CheckAlarm()
    {
        
        if (pathBar.fillAmount > alarmSlider.value)
        {
            SetBlinking(true);
        }
        else
        {
            SetBlinking(false);
        }
    }

    public void SetBlinking(bool value)
    {
        isBlinking = value;

        if (!value)
        {
            time = 0f;
            Color c = alarmBulb.color;
            c.a = 0f;
            alarmBulb.color = c;
        }
    }
}
