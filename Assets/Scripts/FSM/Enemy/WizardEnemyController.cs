using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardEnemyController : RangedEnemyController
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAttackAnimation()
    {
        animator?.SetTrigger("Attack");
    }

    public void PlayDieAnimation()
    {
        animator?.SetTrigger("Die");
    }

    public void SetSpeed(float value)
    {
        animator?.SetFloat("Speed", value);
    }
}


