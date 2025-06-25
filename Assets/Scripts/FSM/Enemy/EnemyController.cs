using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public List<Transform> patrolPoints;
    public float speed = 2f;
    public float detectionRange = 5f;
    public float maxChaseDistance = 10f;
    public Transform player;

    private StateMachine stateMachine;

    private void Start()
    {
        stateMachine = new StateMachine();

        stateMachine.AddState(new EnemyIdleState(this));
        stateMachine.AddState(new EnemyPatrolState(this, stateMachine));
        stateMachine.AddState(new EnemyChaseState(this));
        stateMachine.AddState(new EnemyAttackState(this));
        stateMachine.AddState(new EnemyDeadState(this));

        stateMachine.ChangeState(StateType.Patrol);
    }

    private void Update()
    {
        stateMachine.Update();
    }
    public StateMachine GetStateMachine() => stateMachine;
    public void ChangeState(StateType type) => stateMachine.ChangeState(type);

    public bool PlayerInRange()
    {
        if (player == null) return false;
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    public void MoveTo(Vector3 target)
    {
        Vector3 dir = (target - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
    }

    public bool PlayerTooFar()
    {
        if (player == null) return true;
        return Vector3.Distance(transform.position, player.position) > maxChaseDistance;
    }
}
