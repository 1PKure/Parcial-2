using UnityEngine;
public class EnemyIdleState : State
{
    public override StateType StateType => StateType.Idle;

    private EnemyController enemy;

    public EnemyIdleState(EnemyController enemy) : base(enemy.gameObject, enemy.GetStateMachine())
    {
        this.enemy = enemy;
    }

    public override void Enter() { }
    public override void Update() { }
    public override void Exit() { }
}

