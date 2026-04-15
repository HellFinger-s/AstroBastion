using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WaveInfoShow : MonoBehaviour
{
    public TMP_Text waveInfoText;

    private RectTransform rectTransform;

    private float timer;

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 0.2f)
        {
            waveInfoText.text = "";
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void ShowWaveInfo(string text, Vector2 newPosition)
    {
        waveInfoText.text = text + "Press ENTER to skip preparation";
        //rectTransform.position = newPosition;
        timer = 0f;
    }
}
