using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPositionOnPath : MonoBehaviour
{
    [SerializeField]
    private List<PathPassingVisualizer> pathBars = new List<PathPassingVisualizer> { };

    public float updateInterval = 0.1f;

    private bool barsEnabled = false;
    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > updateInterval)
        {
            UpdateBars();
            timer = 0f;
        }
    }

    public void UpdateBars()
    {
        float[] pathsPassing = LUbus.GetInstance().GetPathsPassing();
        if (!barsEnabled)
        {
            EnableBars(pathsPassing);
        }
        
        for (int i = 0; i < pathBars.Count; i++)
        {
            if (i < pathsPassing.Length && pathsPassing[i] > 0)
            {
                pathBars[i].UpdateInformation(pathsPassing[i]);
            }
        }
    }

    private void EnableBars(float[] pathsPassing)
    {
        for (int i = 0; i < pathsPassing.Length; i++)
        {
            if (i < pathBars.Count && pathsPassing[i] > -1f)
            {
                pathBars[i].EnablePath();
            }
            else
            {
                //Debug.LogError(i);
                //pathBars[i].DisablePath();
            }
        }
        barsEnabled = true;
    }
}
