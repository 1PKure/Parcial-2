using System.Collections.Generic;
using UnityEngine;


public class PlayerController2 : Person
{
    [Header("Movement")]
    [SerializeField] private float maxAngleMovement = 30f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float jumpCooldown = 0.3f;
    [SerializeField] private int maxJumpCount = 1;
    private int currentJumpCount = 0;


    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceSteps;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] float maxTimeAudio = 0.7f;
    [SerializeField] float timeAudio = 0;

    [Header("Camera")]
    [SerializeField] public Transform firstPersonCameraTransform;
    [SerializeField] public Transform thirdPersonCameraTransform;
    [SerializeField] public Transform pivot;
    [SerializeField] public Transform cameraTransform;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private float lastJumpTime = -10f;

    public bool isFirstPerson = true;
    private bool isGrounded;
    private Rigidbody rb;
    private bool isMoving = false;
    
    private StateMachine stateMachine;
    private float mouseSensitivity = 200f;
    private float rotationY = 0f;
    private float rotationX = 0f;
    private float maxAngle = 80f;
    private bool initialized = false;
    private bool JumpPressed => Input.GetKeyDown(KeyCode.Space);
    public bool IsPossessed { get; set; }
    private void Start()
    {
        Initialize();
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;
        Cursor.lockState = CursorLockMode.Locked;
        stateMachine.Update();
        HandleJump();
        HandleRotation();
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;

            cameraTransform = isFirstPerson ? firstPersonCameraTransform : thirdPersonCameraTransform;
        }
    }
    public void Move(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

        if (CanMove(moveDir))
        {
            PlayAudio();
            rb.velocity = velocity;
        }
    }
    public void ResetRotation()
    {
        rotationY = transform.eulerAngles.y;
        pivot.rotation = Quaternion.Euler(0f, rotationY, 0f);

        rotationX = 0f;
        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        cameraTransform = isFirstPerson ? firstPersonCameraTransform : thirdPersonCameraTransform;
    }

    private void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        pivot.rotation = Quaternion.Euler(0f, rotationY, 0f);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -maxAngle, maxAngle);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0f);
    }

    private void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && rb.velocity.y < 0)
        {
            rb.velocity = new Vector3(rb.velocity.x, -2f, rb.velocity.z);
            currentJumpCount = 0;
        }

        if (JumpPressed && currentJumpCount < maxJumpCount && Time.time > lastJumpTime + jumpCooldown)
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            lastJumpTime = Time.time;
            currentJumpCount++;
            AudioManager.Instance?.PlayJump();
        }

        rb.velocity += new Vector3(0, gravity * Time.deltaTime, 0);
    }


    /*
    private bool CanMove(Vector3 moveDir)
    {
        Terrain terrain = Terrain.activeTerrain;
        Vector3 relativePos = GetMapPos();
        Vector3 normal = terrain.terrainData.GetInterpolatedNormal(relativePos.x, relativePos.z);
        float angle = Vector3.Angle(normal, Vector3.up);

        float currentHeight = terrain.SampleHeight(rb.position);
        float nextHeight = terrain.SampleHeight(rb.position + moveDir * 5);

        if (angle > maxAngleMovement && nextHeight > currentHeight)
            return false;
        return true;
    }
    */
    private bool CanMove(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.0001f) return true;

        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out RaycastHit groundHit, 2f, groundMask))
        {
            float groundAngle = Vector3.Angle(groundHit.normal, Vector3.up);

            if (groundAngle > maxAngleMovement)
            {
                Vector3 downhill = Vector3.ProjectOnPlane(Vector3.down, groundHit.normal).normalized;
                if (Vector3.Dot(moveDir.normalized, downhill) <= 0.05f)
                    return false;
            }
        }

        float castDistance = 0.6f;
        float radius = 0.35f;

        Vector3 p1 = transform.position + Vector3.up * 0.2f;
        Vector3 p2 = transform.position + Vector3.up * 1.6f;

        if (Physics.CapsuleCast(p1, p2, radius, moveDir.normalized, out RaycastHit hit, castDistance, groundMask))
        {
            float hitAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (hitAngle > maxAngleMovement)
                return false;
        }

        return true;
    }


    private void PlayAudio()
    {
        if (!isGrounded) return;

        Vector3 input = GetInputDirection();
        if (input.magnitude < 0.1f) return;

        timeAudio += Time.deltaTime;
        if (timeAudio < maxTimeAudio) return;

        timeAudio = 0;

        Terrain terrain = Terrain.activeTerrain;
        Vector3 pos = GetMapPos();

        int mapX = Mathf.FloorToInt(pos.x * terrain.terrainData.alphamapWidth);
        int mapZ = Mathf.FloorToInt(pos.z * terrain.terrainData.alphamapHeight);

        float[,,] splatmapData = terrain.terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        int maxTextures = terrain.terrainData.alphamapLayers;

        float maxValue = 0;
        int index = 0;
        for (int i = 0; i < maxTextures; i++)
        {
            if (splatmapData[0, 0, i] > maxValue)
            {
                maxValue = splatmapData[0, 0, i];
                index = i;
            }
        }

        if (index < clips.Count)
        {
            audioSourceSteps.clip = clips[index];
            audioSourceSteps.Play();
        }
    }

    private Vector3 GetMapPos()
    {
        Vector3 pos = rb.position;
        Terrain terrain = Terrain.activeTerrain;

        return new Vector3((pos.x - terrain.transform.position.x) / terrain.terrainData.size.x,
                           0,
                           (pos.z - terrain.transform.position.z) / terrain.terrainData.size.z);
    }

    public void ChangeState(StateType newState)
    {
        stateMachine.ChangeState(newState);
    }

    public StateMachine GetStateMachine()
    {
        return stateMachine;
    }

    public Vector3 GetInputDirection()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0f;
        camRight.Normalize();

        Vector3 moveDir = (camForward * vertical + camRight * horizontal).normalized;
        return moveDir;
    }

    public void SetCameraTarget(Transform newTarget)
    {
        cameraTransform = newTarget;
    }

    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        stateMachine = new StateMachine();
        stateMachine.AddState(new PlayerIdleState(this));
        stateMachine.AddState(new PlayerMoveState(this));
        stateMachine.AddState(new PlayerPossessedState(this));
        stateMachine.ChangeState(StateType.Idle);
        cameraTransform = isFirstPerson ? firstPersonCameraTransform : thirdPersonCameraTransform;
    }

    public override void EnableControl()
    {
        enabled = true;
        foreach (var comp in GetComponents<MonoBehaviour>())
            if (comp != this) comp.enabled = true;
    }

    public override void DisableControl()
    {
        foreach (var comp in GetComponents<MonoBehaviour>())
        {
            if (comp != this && !(comp is GhostController))
                comp.enabled = false;
        }

        enabled = false;
    }

    public override Transform GetCameraTarget()
    {
        return transform;
    }

    public void SetupFromTemplate(PlayerController2 template)
    {
        this.maxAngleMovement = template.maxAngleMovement;
        this.moveSpeed = template.moveSpeed;
        this.groundMask = template.groundMask;

        this.jumpHeight = template.jumpHeight;
        this.gravity = template.gravity;
        this.groundDistance = template.groundDistance;

        this.maxTimeAudio = template.maxTimeAudio;
        this.clips = new List<AudioClip>(template.clips);
        this.audioSourceSteps = template.audioSourceSteps;
        this.audioSourceMusic = template.audioSourceMusic;
    }

    public void InjectRuntimeRefs(Transform pivotRef, Transform camRef, Transform groundCheckRef)
    {
        pivot = pivotRef;
        cameraTransform = camRef;
        firstPersonCameraTransform = camRef;
        thirdPersonCameraTransform = camRef;
        groundCheck = groundCheckRef;
    }
}
