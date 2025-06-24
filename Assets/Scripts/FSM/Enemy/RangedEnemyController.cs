using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private Transform player;

    private float lastAttackTime;
    private StateMachine stateMachine;

    private void Start()
    {
        stateMachine = new StateMachine();

        stateMachine.AddState(new EnemyPatrolState(this));
        stateMachine.AddState(new EnemyRangedAttackState(this));
        stateMachine.ChangeState(StateType.Patrol);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    public void ChangeState(StateType type)
    {
        stateMachine.ChangeState(type);
    }

    public StateMachine GetStateMachine()
    {
        return stateMachine;
    }

    public bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    public void LookAtPlayer()
    {
        if (player == null) return;
        Vector3 lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        transform.forward = lookDir;
    }

    public void Shoot()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            lastAttackTime = Time.time;
        }
    }
}

