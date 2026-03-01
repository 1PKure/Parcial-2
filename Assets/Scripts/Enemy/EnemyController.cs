using UnityEngine;
using System.Collections.Generic;

public class EnemyController : Person
{
    [Header("Patrol (Opcional por puntos)")]
    public List<Transform> patrolPoints;

    [Header("Patrol (Sin puntos - Wander)")]
    [SerializeField] private bool useWanderWhenNoPoints = true;
    [SerializeField] private float wanderRadius = 6f;
    [SerializeField] private float wanderRepathTime = 2f; 
    [SerializeField] private float arriveDistance = 0.6f;

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

    private Vector3 homePosition;
    private Vector3 currentPatrolTarget;
    private float nextRepathTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        homePosition = transform.position;   
        AdjustToGround();
        homePosition = transform.position;   

        Initialize();
        initialized = true;

        if (ShouldWander())
            PickNewWanderTarget(true);
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

    public bool HasPatrolPoints()
    {
        return patrolPoints != null && patrolPoints.Count > 0 && patrolPoints[0] != null;
    }

    public bool ShouldWander()
    {
        return useWanderWhenNoPoints && !HasPatrolPoints();
    }

    public Vector3 GetPatrolTarget()
    {
        if (ShouldWander())
        {
            if (Time.time >= nextRepathTime)
                PickNewWanderTarget(false);

            return currentPatrolTarget;
        }
        return transform.position;
    }

    public bool ArrivedTo(Vector3 target)
    {
        Vector3 a = transform.position; a.y = 0;
        Vector3 b = target; b.y = 0;
        return Vector3.Distance(a, b) <= arriveDistance;
    }

    public void PickNewWanderTarget(bool immediate)
    {
        if (!ShouldWander()) return;

        if (immediate)
            nextRepathTime = Time.time;
        else
            nextRepathTime = Time.time + wanderRepathTime;
        for (int i = 0; i < 8; i++)
        {
            Vector2 rnd = Random.insideUnitCircle * wanderRadius;
            Vector3 candidate = homePosition + new Vector3(rnd.x, 0f, rnd.y);

            Vector3 origin = candidate + Vector3.up * 5f;
            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 20f, groundMask))
            {
                currentPatrolTarget = hit.point + Vector3.up * heightOffset;
                return;
            }
        }

        currentPatrolTarget = homePosition;
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