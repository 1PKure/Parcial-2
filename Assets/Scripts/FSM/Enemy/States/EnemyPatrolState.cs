using UnityEngine;

public class EnemyPatrolState : State
{
    public override StateType StateType => StateType.Patrol;

    private EnemyController enemy;
    private int currentPoint = 0;

    public EnemyPatrolState(EnemyController enemy) : base(enemy.gameObject, enemy.GetStateMachine())
    {
        this.enemy = enemy;
    }

    public override void Enter() { }

    public override void Update()
    {
        if (enemy.patrolPoints.Count == 0) return;

        Transform target = enemy.patrolPoints[currentPoint];
        enemy.MoveTo(target.position);

        if (Vector3.Distance(enemy.transform.position, target.position) < 0.5f)
            currentPoint = (currentPoint + 1) % enemy.patrolPoints.Count;

        if (enemy.PlayerInRange())
        { 
            enemy.GetStateMachine().ChangeState(StateType.Chase);
        }
    }

    public override void Exit() { }
}
