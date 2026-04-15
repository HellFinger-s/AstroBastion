using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class WavePanel : MonoBehaviour
{
    public TMP_Text waveText;

    public void UpdateWaveNumber(int waveNumber, int wavesCount)
    {
        waveText.text = string.Format("{0}/{1}", waveNumber, wavesCount);
    }
}
