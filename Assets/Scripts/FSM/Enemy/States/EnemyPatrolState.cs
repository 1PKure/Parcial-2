using UnityEngine;

public class EnemyPatrolState : State
{
    public override StateType StateType => StateType.Patrol;

    private MonoBehaviour enemyBase;
    private int currentPoint = 0;
    private WizardEnemyController rangedEnemy;

    public EnemyPatrolState(MonoBehaviour enemy, StateMachine sm)
        : base(enemy.gameObject, sm)
    {
        this.enemyBase = enemy;
    }

    public override void Enter() { }

    public override void Update()
    {
        if (enemyBase is EnemyController meleeEnemy)
        {
            if (meleeEnemy.PlayerInRange())
            {
                meleeEnemy.ChangeState(StateType.Chase);
                return;
            }

            if (meleeEnemy.ShouldWander())
            {
                Vector3 target = meleeEnemy.GetPatrolTarget();
                meleeEnemy.MoveTo(target);

                if (meleeEnemy.ArrivedTo(target))
                    meleeEnemy.PickNewWanderTarget(true);

                return;
            }

            if (meleeEnemy.patrolPoints == null || meleeEnemy.patrolPoints.Count == 0)
                return;

            if (meleeEnemy.patrolPoints[currentPoint] == null)
            {
                currentPoint = (currentPoint + 1) % meleeEnemy.patrolPoints.Count;
                return;
            }

            Vector3 pointTarget = meleeEnemy.patrolPoints[currentPoint].position;
            meleeEnemy.MoveTo(pointTarget);

            if (Vector3.Distance(meleeEnemy.transform.position, pointTarget) < 0.5f)
                currentPoint = (currentPoint + 1) % meleeEnemy.patrolPoints.Count;
        }
        else if (enemyBase is RangedEnemyController rangedEnemy)
        {
            if (rangedEnemy.PlayerInRange())
            {
                rangedEnemy.ChangeState(StateType.Attack);
                return;
            }

            if (rangedEnemy is WizardEnemyController wizard)
                wizard.SetSpeed(1f);
        }
    }

    public override void Exit()
    {
        if (rangedEnemy is WizardEnemyController wizard)
            wizard.SetSpeed(0f);
    }
}