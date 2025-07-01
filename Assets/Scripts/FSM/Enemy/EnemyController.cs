using UnityEngine;
using System.Collections.Generic;

public class EnemyController : Person
{
    public List<Transform> patrolPoints;
    public float speed = 2f;
    public float detectionRange = 5f;
    public float maxChaseDistance = 10f;
    public Transform player;
    private bool initialized = false;

    private StateMachine stateMachine;

    private void Start()
    {
        Initialize();
        initialized = true;
    }
    private void Update()
    {
        if (!initialized) return;
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

    public override void Initialize()
    {
        stateMachine = new StateMachine();

        stateMachine.AddState(new EnemyIdleState(this));
        stateMachine.AddState(new EnemyPatrolState(this, stateMachine));
        stateMachine.AddState(new EnemyChaseState(this));
        stateMachine.AddState(new EnemyAttackState(this));
        stateMachine.AddState(new EnemyDeadState(this));

        stateMachine.ChangeState(StateType.Patrol);
    }

    public override void EnableControl()
    {
        enabled = true;
        foreach (var comp in GetComponents<MonoBehaviour>())
            if (comp != this) comp.enabled = true;
    }

    public override void DisableControl()
    {
        enabled = false;
        foreach (var comp in GetComponents<MonoBehaviour>())
            if (comp != this) comp.enabled = false;
    }

    public override Transform GetCameraTarget()
    {
        return transform; // Si no hay cámara asociada, se devuelve el propio transform
    }
}
