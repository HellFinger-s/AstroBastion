using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EnemyInfo : MonoBehaviour
{
    public Slider hpSlider;
    public TMP_Text hpValueText;
    public Slider shieldSlider;
    public TMP_Text shieldValueText;
    public TMP_Text enemyName;
    public TMP_Text resists;
    public float changingSpeed = 10f;

    private float currentHpValue = 0f;
    private float currentShieldValue = 0f;
    private float targetHpValue;
    private float targetShieldValue;

    private void Update()
    {
        currentHpValue = Mathf.Lerp(currentHpValue, targetHpValue, changingSpeed * Time.deltaTime);
        currentShieldValue = Mathf.Lerp(currentShieldValue, targetShieldValue, changingSpeed * Time.deltaTime);

        hpSlider.value = currentHpValue;
        shieldSlider.value = currentShieldValue;
    }


    public void ShowInfo(BaseEnemy enemy)
    {
        targetHpValue = enemy.GetCurrentHealth();
        currentHpValue = targetHpValue;
        hpSlider.maxValue = enemy.GetScaledMaxHealth();
        hpSlider.gameObject.SetActive(true);
        hpValueText.text = string.Format("{0:f1}/{1:f1}", enemy.GetCurrentHealth(), enemy.GetScaledMaxHealth());
        if (enemy.shield.maxValue > 0)
        {
            shieldSlider.gameObject.SetActive(true);
            shieldSlider.maxValue = enemy.shield.maxValue;
            shieldValueText.text = string.Format("{0:f1}/{1:f1}", enemy.shield.currentValue, enemy.shield.maxValue);
            targetShieldValue = enemy.shield.currentValue; // GetShieldValue()
        }
        else
        {
            shieldSlider.gameObject.SetActive(false);
        }
        enemyName.text = Localizer.GetInstance().Localize(enemy.keyName.ToString());

        resists.text = string.Format("{0:f1} <sprite name=\"KineticResist\">        {1:f1} <sprite name=\"EnergyResist\">                   +{2:f1} <sprite name=\"Reward\">", enemy.kineticResist, enemy.energyResist, enemy.GetScaledReward());
    }

    public void DisableInfo()
    {
        hpSlider.gameObject.SetActive(false);
        enemyName.text = "";
        resists.text = "";
        shieldSlider.gameObject.SetActive(false);
    }
}
