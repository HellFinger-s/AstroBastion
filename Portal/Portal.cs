using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Portal : MonoBehaviour
{
    private System.Random rnd = new System.Random();

    public Transform formationStartPlace;

    private Dictionary<EnemyTypes, int> waveInfo = new Dictionary<EnemyTypes, int> { };

    string text = "";

    public Vector3 GetOffset(int radius = 30)
    {
        return transform.right * rnd.Next(-radius, radius) + transform.forward * rnd.Next(-radius, radius);
    }

    public void AddWaveInfo(EnemyTypes type, int count)
    {
        if (waveInfo.ContainsKey(type))
        {
            waveInfo[type] += count;
        }
        else
        {
            waveInfo.Add(type, count);
        }
    }

    public void ClearInfo()
    {
        waveInfo.Clear();
    }

    public void ShowWaveInfo(Vector2 position)
    {
        LUbus.GetInstance().ShowWaveInfo(text, position);
    }

    public void ClearText()
    {
        text = "";
    }

    public void CombineText()
    {
        foreach(var key in waveInfo.Keys)
        {
            text += string.Format("{0} x{1} \n", Localizer.GetInstance().Localize(key.ToString()), waveInfo[key]);
        }
    }
}
