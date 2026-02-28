using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemyController : MonoBehaviour
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private Transform player;

    [Header("Aim")]
    [SerializeField] private float aimHeightOffset = 1.2f;


    private float lastAttackTime;
    private StateMachine stateMachine;
    private void Start()
    {
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        stateMachine = new StateMachine();

        stateMachine.AddState(new EnemyPatrolState(this, stateMachine));
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

        Vector3 lookDir = (player.position - transform.position);
        lookDir.y = 0f;
        if (lookDir.sqrMagnitude > 0.001f)
            transform.forward = lookDir.normalized;
    }

    public void Shoot()
    {
        if (player == null || firePoint == null || projectilePrefab == null) return;
        if (Time.time < lastAttackTime + attackCooldown) return;

        Vector3 targetPos = player.position + Vector3.up * aimHeightOffset;
        Vector3 dir = (targetPos - firePoint.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject go = Instantiate(projectilePrefab, firePoint.position, rot);

        var proj = go.GetComponent<EnemyProjectile>();
        if (proj != null)
        {
            proj.Init(dir, proj.damage, proj.speed);
        }
        else
        {
            var rb = go.GetComponent<Rigidbody>();
            if (rb != null) rb.velocity = dir * 15f;
        }

        lastAttackTime = Time.time;
    }
}

