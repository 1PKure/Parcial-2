using UnityEngine;

public class EnemyPatrolState : State
{
    public override StateType StateType => StateType.Patrol;

    private MonoBehaviour enemyBase;
    private int currentPoint = 0;

    public EnemyPatrolState(MonoBehaviour enemy)
        : base(enemy.gameObject, enemy.GetComponent<StateMachine>())
    {
        enemyBase = enemy;
    }

    public override void Enter() { }

    public override void Update()
    {
        if (enemyBase is EnemyController enemy)
        {
            if (enemy.PlayerInRange())
            {
                enemy.ChangeState(StateType.Chase);
                return;
            }

            if (enemy.patrolPoints.Count == 0) return;

            Vector3 target = enemy.patrolPoints[currentPoint].position;
            enemy.MoveTo(target);

            if (Vector3.Distance(enemy.transform.position, target) < 0.5f)
                currentPoint = (currentPoint + 1) % enemy.patrolPoints.Count;
        }

        else if (enemyBase is RangedEnemyController rangedEnemy)
        {
            if (rangedEnemy.PlayerInRange())
            {
                rangedEnemy.ChangeState(StateType.Attack);
                return;
            }
        }
    }

    public override void Exit() { }
}
