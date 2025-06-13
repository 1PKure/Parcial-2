using UnityEngine;
using System.Collections.Generic;
using Clase10;

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
        stateMachine.ChangeState(new EnemyPatrolState(this));
    }

    private void Update()
    {
        stateMachine.Update();
    }
    public StateMachine GetStateMachine()
    {
        return stateMachine;
    }

    public void ChangeState(State newState) => stateMachine.ChangeState(newState);

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
