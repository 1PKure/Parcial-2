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
    private Rigidbody rb;

    [Header("Terreno")]
    [SerializeField] private LayerMask groundMask = default;
    [SerializeField] private float heightOffset = 0.5f;
    private StateMachine stateMachine;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        AdjustToGround();
        Initialize();
        initialized = true;
    }
    private void Update()
    {
        if (!initialized) return;
        stateMachine.Update();
    }
    private void AdjustToGround()
    {
        Vector3 origin = transform.position + Vector3.up;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 5f, groundMask))
        {
            Vector3 pos = transform.position;
            pos.y = hit.point.y + heightOffset;
            transform.position = pos;
        }
    }
    public StateMachine GetStateMachine() => stateMachine;
    public void ChangeState(StateType type) => stateMachine.ChangeState(type);

    public bool PlayerInRange()
    {
        if (player.TryGetComponent<PlayerController2>(out var pc) && pc.IsPossessed)
            return false;
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    public bool PlayerTooFar()
    {
        if (player.TryGetComponent<PlayerController2>(out var pc) && pc.IsPossessed)
            return true;
        return Vector3.Distance(transform.position, player.position) > maxChaseDistance;
    }

    public void MoveTo(Vector3 target)
    {
        Vector3 dir = (target - transform.position);
        dir.y = 0;
        float distance = dir.magnitude;

        if (distance < 0.1f) return;

        dir.Normalize();

        if (dir != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }

        Vector3 newPosition = transform.position + dir * speed * Time.deltaTime;

        rb.MovePosition(newPosition);
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
        return transform;
    }
}
