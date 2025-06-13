using Clase10;
using UnityEngine;

public class EnemyChaseState : State
{
    private EnemyController enemy;

    public EnemyChaseState(EnemyController enemyController)
    {
        enemy = enemyController;
    }

    public override void Enter() { }

    public override void Update()
    {
        if (!enemy.PlayerInRange())
        {
            if (enemy.PlayerTooFar())
            {
                enemy.ChangeState(new EnemyPatrolState(enemy));
                return;
            }
        }

        enemy.MoveTo(enemy.player.position);

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) < 1.5f)
        {
            enemy.ChangeState(new EnemyAttackState(enemy));
        }
    }

    public override void Exit() { }
}
