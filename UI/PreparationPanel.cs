using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PreparationPanel : MonoBehaviour
{
    public TMP_Text preparationSeconds;
    public GameObject icon;

    private float timer = 0f;
    private bool preparation = false;

    private void Update()
    {
        if (preparation)
        {
            timer -= Time.deltaTime;
            preparationSeconds.text = string.Format("{0:f1}", timer);
            if (timer < 0.1)
            {
                preparation = false;
                timer = 0f;
                preparationSeconds.text = "";
                icon.SetActive(false);
            }
        }
    }

    public void StartPreparation(float seconds)
    {
        timer = seconds;
        preparation = true;
        icon.SetActive(true);
    }

    public void PreparationInterrupted()
    {
        preparation = false;
        timer = 0f;
        preparationSeconds.text = "";
        icon.SetActive(false);
    }
}
