using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitstopTower : BaseTower, IHealer
{
    public GameObject booster;

    public float boosterTimeout;

    public void ReloadBooster()
    {
        StartCoroutine(wait());
    }

    private IEnumerator wait()
    {
        yield return new WaitForSeconds(boosterTimeout);
        booster.SetActive(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        HandleOnTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleOnTriggerExit(other);
    }

    public override Dictionary<string, string> GetSubstitutions(IButtonTextTemplate template)
    {
        Dictionary<string, string> substitutions = new Dictionary<string, string> { };

        if (template is BuildableTextTemplate)
        {
            substitutions.Add(TextMarks.Timeout.ToString(), boosterTimeout.ToString());
            substitutions.Add(TextMarks.HP.ToString(), maxHealth.ToString());
        }

        return substitutions;
    }
}
