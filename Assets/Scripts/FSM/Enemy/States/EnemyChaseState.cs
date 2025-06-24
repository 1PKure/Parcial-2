using UnityEngine;

public class EnemyChaseState : State
{
    public override StateType StateType => StateType.Chase;

    private EnemyController enemy;

    public EnemyChaseState(EnemyController enemy) : base(enemy.gameObject, enemy.GetStateMachine())
    {
        this.enemy = enemy;
    }

    public override void Enter() { }

    public override void Update()
    {
        if (!enemy.PlayerInRange())
        {
            if (enemy.PlayerTooFar())
            {
                enemy.GetStateMachine().ChangeState(StateType.Patrol);
                return;
            }
        }

        enemy.MoveTo(enemy.player.position);

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 1.5f)
            enemy.GetStateMachine().ChangeState(StateType.Attack);
    }

    public override void Exit() { }
}
