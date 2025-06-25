using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWizardAttackState : State
{
    public override StateType StateType => StateType.Attack;

    private WizardEnemyController enemy;

    public EnemyWizardAttackState(WizardEnemyController enemy)
        : base(enemy.gameObject, enemy.GetStateMachine())
    {
        this.enemy = enemy;
    }

    public override void Enter()
    {
        enemy.SetSpeed(0f);
        enemy.PlayAttackAnimation();
    }

    public override void Update()
    {
        if (!enemy.PlayerInRange())
        {
            enemy.ChangeState(StateType.Patrol);
            return;
        }

        enemy.LookAtPlayer();
        enemy.Shoot();
    }

    public override void Exit() { }
}

