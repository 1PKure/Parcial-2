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

            if (meleeEnemy.patrolPoints.Count == 0) return;

            Vector3 target = meleeEnemy.patrolPoints[currentPoint].position;
            meleeEnemy.MoveTo(target);

            if (Vector3.Distance(meleeEnemy.transform.position, target) < 0.5f)
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
