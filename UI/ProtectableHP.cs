using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProtectableHP : MonoBehaviour
{
    [SerializeField]
    private TMP_Text text;

    [SerializeField]
    private Animator animator;

    public void UpdateHP(int value, bool init=false)
    {
        
        text.text = string.Format("{0}<sprite name=\"PlanetHP\">", value);
        if (!init)
        {
            animator.SetTrigger("PlanetDamaged");
        }

    }
}
