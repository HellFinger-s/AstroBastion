using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RespawnTimeout : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    private float startValue;
    private bool count = false;
    
    void Update()
    {
        if (count)
        {
            startValue -= Time.deltaTime;
            text.text = string.Format("{0:f2}", startValue);
            if (startValue < 1e-3)
            {
                text.text = "";
                count = false;
            }
        }
    }

    public void StartCount(float value)
    {
        count = true;
        startValue = value;
    }
}
