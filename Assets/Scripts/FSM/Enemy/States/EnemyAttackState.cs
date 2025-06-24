using UnityEngine;

public class EnemyAttackState : State
{
    public override StateType StateType => StateType.Attack;

    private EnemyController enemy;
    private float attackCooldown = 1.5f;
    private float lastAttackTime = 0;
    private PlayerHealth playerHealth;

    public EnemyAttackState(EnemyController enemy) : base(enemy.gameObject, enemy.GetStateMachine())
    {
        this.enemy = enemy;
        if (enemy.player != null)
            playerHealth = enemy.player.GetComponent<PlayerHealth>();
    }

    public override void Enter()
    {
        lastAttackTime = Time.time - attackCooldown;
    }

    public override void Update()
    {
        if (!enemy.PlayerInRange())
        {
            enemy.GetStateMachine().ChangeState(StateType.Chase);
            return;
        }

        if (Time.time >= lastAttackTime + attackCooldown)
        {
            lastAttackTime = Time.time;
            playerHealth?.TakeDamage(10);
        }
    }

    public override void Exit() { }
}
