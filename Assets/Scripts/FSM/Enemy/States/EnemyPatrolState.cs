using UnityEngine;

public class EnemyPatrolState : State
{
    private EnemyController enemy;
    private int currentPoint = 0;

    public EnemyPatrolState(EnemyController enemyController)
        : base(enemyController.gameObject, enemyController.GetStateMachine())
    {
        enemy = enemyController;
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
            enemy.ChangeState(new EnemyChaseState(enemy));
    }

    public override void Exit() { }
}
