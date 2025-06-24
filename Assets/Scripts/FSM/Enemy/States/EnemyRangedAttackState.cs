using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedAttackState : State
{
    public override StateType StateType => StateType.Attack;

    private RangedEnemyController enemy;

    public EnemyRangedAttackState(RangedEnemyController enemy)
        : base(enemy.gameObject, enemy.GetStateMachine())
    {
        this.enemy = enemy;
    }

    public override void Enter() { }

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

