using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class ResourcePanel : MonoBehaviour
{
    public TMP_Text resourceText;

    private void Start()
    {
        UpdateValue(LUbus.GetInstance().GetResources());
    }

    public void UpdateValue(float value)
    {
        resourceText.text = String.Format("{0:f1}", value);
    }
}
