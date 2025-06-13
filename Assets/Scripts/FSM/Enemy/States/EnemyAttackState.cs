using Clase10;
using UnityEngine;

public class EnemyAttackState : State
{
    private EnemyController enemy;
    private float attackCooldown = 1.5f;
    private float lastAttackTime = 0;
    private PlayerHealth playerHealth;

    public EnemyAttackState(EnemyController enemyController)
    {
        enemy = enemyController;
        if (enemy.player != null)
            playerHealth = enemy.player.GetComponent<PlayerHealth>();
    }

    public override void Enter() { lastAttackTime = Time.time - attackCooldown; }

    public override void Update()
    {
        if (!enemy.PlayerInRange())
        {
            enemy.ChangeState(new EnemyPatrolState(enemy));
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Debug.Log("Enemy attacks player!");
            lastAttackTime = Time.time;
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
            }
        }
    }

    public override void Exit() { }
}
