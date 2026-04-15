using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUnderAttackIndicator : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    public void Animate()
    {
        animator.SetTrigger("TowerAttacked");
    }
}
