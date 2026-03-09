using System;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController2 : Person
{
    [Header("Config")]
    [SerializeField] private PlayerControllerConfigSO config;

    [Header("Movement")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    private int currentJumpCount = 0;
    [SerializeField] private float stepDistanceWalk = 1.8f;
    [SerializeField] private float stepDistanceSprint = 1.2f;
    [SerializeField] private float minSpeedForSteps = 0.2f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSourceSteps;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] float maxTimeAudio = 0.7f;

    [Header("Camera")]
    [SerializeField] public Transform firstPersonCameraTransform;
    [SerializeField] public Transform thirdPersonCameraTransform;
    [SerializeField] public Transform pivot;
    [SerializeField] public Transform cameraTransform;

    [SerializeField] private float slopeCheckDistance = 1.2f;
    [SerializeField] private float steepSlideGravityMultiplier = 2.2f;
    private bool isOnSteepSlope;
    private Vector3 steepSlopeDownDir;

    [System.Serializable]
    public class FootstepLayerClip
    {
        public TerrainLayer layer;
        public AudioClip clip;
    }

    [Header("Stamina / Sprint")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

    public bool isFirstPerson = true;
    private bool isGrounded;
    private Rigidbody rb;
    private float lastJumpTime = -10f;
    private StateMachine stateMachine;
    private float rotationY = 0f;
    private float rotationX = 0f;
    private bool initialized = false;
    private Vector3 lastStepPosition;
    private float stepAccumulated;
    private float stamina;
    private float lastSprintTime;
    private bool isSprinting;
    public event Action<float> OnStaminaNormalizedChanged;
    public float StaminaNormalized => (config.maxStamina <= 0f) ? 0f : Mathf.Clamp01(stamina / config.maxStamina);
    public bool IsSprinting => isSprinting;

    private bool SprintHeld => Input.GetKey(sprintKey);
    private void NotifyStaminaChanged()
    {
        OnStaminaNormalizedChanged?.Invoke(StaminaNormalized);
    }
    private bool JumpPressed => Input.GetKeyDown(KeyCode.Space);
    public bool IsPossessed { get; set; }
    private void Awake()
    {
        if (config == null)
            config = Resources.Load<PlayerControllerConfigSO>("PlayerControllerConfig");
    }
    private void Start()
    {
        lastStepPosition = transform.position;
        stepAccumulated = 0f;
        if (config == null)
        {
            Debug.LogError("PlayerController2: Falta asignar PlayerControllerConfigSO.");
            enabled = false;
            return;
        }
        stamina = config.maxStamina;
        NotifyStaminaChanged();
        EnsureFootstepAudioSource();

        Initialize();
        initialized = true;
    }

    private void Update()
    {
        if (!initialized) return;

        if (Time.timeScale == 0f) return;

        if (!Cursor.visible)
            Cursor.lockState = CursorLockMode.Locked;
        rb.drag = isGrounded ? 8f : 0f;

        UpdateSlopeState();
        stateMachine.Update();
        HandleJump();
        HandleRotation();

        /*
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;

            cameraTransform = isFirstPerson ? firstPersonCameraTransform : thirdPersonCameraTransform;
        }
        */
        TickStamina();
        TickFootsteps();
    }
    private void SetStamina(float value)
    {
        float clamped = Mathf.Clamp(value, 0f, config.maxStamina);
        if (Mathf.Approximately(clamped, stamina)) return;

        stamina = clamped;
        NotifyStaminaChanged();
    }
    public void Move(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.001f)
        {
            rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
            return;
        }

        if (moveDir.sqrMagnitude > 1f)
            moveDir.Normalize();

        float speed = config.moveSpeed;

        if (config.enableStamina && isSprinting)
            speed *= config.sprintMultiplier;
        else if (!config.enableStamina && SprintHeld)
            speed *= config.sprintMultiplier;

        Vector3 currentVel = rb.velocity;
        Vector3 desiredHorizontal = new Vector3(moveDir.x * speed, 0f, moveDir.z * speed);

        if (isOnSteepSlope)
        {
            Vector3 downhill = steepSlopeDownDir.normalized;
            float uphillDot = Vector3.Dot(desiredHorizontal.normalized, downhill);

            if (desiredHorizontal.sqrMagnitude > 0.001f && uphillDot <= 0.1f)
            {
                desiredHorizontal = Vector3.zero;
            }

            Vector3 slideVelocity = downhill * speed;
            rb.velocity = new Vector3(slideVelocity.x, currentVel.y, slideVelocity.z);
            return;
        }

        if (CanMove(moveDir))
        {
            rb.velocity = new Vector3(desiredHorizontal.x, currentVel.y, desiredHorizontal.z);
        }
    }
    private void TickStamina()
    {
        if (!config.enableStamina)
        {
            isSprinting = SprintHeld;
            return;
        }

        Vector3 inputDir = GetInputDirection();
        bool hasMoveInput = inputDir.sqrMagnitude > 0.01f;

        bool wantsSprint = SprintHeld && hasMoveInput;

        if (!isSprinting && wantsSprint && stamina < config.minStaminaToStartSprint)
            wantsSprint = false;

        isSprinting = wantsSprint;

        if (isSprinting)
        {
            lastSprintTime = Time.time;
            SetStamina(stamina - config.staminaDrainPerSecond * Time.deltaTime);
            return;
        }

        if (Time.time < lastSprintTime + config.staminaRegenDelay) return;

        SetStamina(stamina + config.staminaRegenPerSecond * Time.deltaTime);
    }

    private void TickFootsteps()
    {
        if (!isGrounded)
        {
            lastStepPosition = transform.position;
            stepAccumulated = 0f;
            return;
        }

        Vector3 input = GetInputDirection();
        if (input.sqrMagnitude < 0.01f)
        {
            lastStepPosition = transform.position;
            stepAccumulated = 0f;
            return;
        }

        Vector3 delta = transform.position - lastStepPosition;
        delta.y = 0f;

        float dist = delta.magnitude;
        if (dist <= 0f)
        {
            lastStepPosition = transform.position;
            return;
        }

        float speed = dist / Mathf.Max(Time.deltaTime, 0.0001f);
        if (speed < minSpeedForSteps)
        {
            lastStepPosition = transform.position;
            stepAccumulated = 0f;
            return;
        }

        stepAccumulated += dist;
        lastStepPosition = transform.position;

        float stepDist = isSprinting ? stepDistanceSprint : stepDistanceWalk;

        while (stepAccumulated >= stepDist)
        {
            stepAccumulated -= stepDist;
            PlayFootstep(speed);
        }
    }

    private void PlayFootstep(float horizontalSpeed)
    {
        if (audioSourceSteps == null) return;
        if (clips == null || clips.Count == 0) return;

        AudioClip clipToPlay = clips[0];

        if (TryGetTerrainUnderfoot(out Terrain terrain, out Vector3 hitPoint) && terrain != null)
        {
            int idx = GetDominantTerrainTextureIndex(terrain, hitPoint);
            idx = Mathf.Clamp(idx, 0, clips.Count - 1);
            clipToPlay = clips[idx];
        }

        audioSourceSteps.PlayOneShot(clipToPlay);
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
        float mouseX = Input.GetAxis("Mouse X") * config.mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * config.mouseSensitivity * Time.deltaTime;

        rotationY += mouseX;
        pivot.rotation = Quaternion.Euler(0f, rotationY, 0f);

        rotationX -= mouseY;
        rotationX = Mathf.Clamp(rotationX, -config.maxLookAngle, config.maxLookAngle);

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0f);
    }

    private void HandleJump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, config.groundCheckRadius, groundMask);

        bool invalidSlopeForJump = IsOnInvalidSlopeForJump();

        if (isGrounded && !invalidSlopeForJump)
        {
            currentJumpCount = 0;
        }

        if (invalidSlopeForJump)
        {
            return;
        }

        if (JumpPressed &&
            currentJumpCount < config.maxJumpCount &&
            Time.time > lastJumpTime + config.jumpCooldown)
        {
            float jumpForce = Mathf.Sqrt(config.jumpHeight * -2f * config.gravity);
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            lastJumpTime = Time.time;
            currentJumpCount++;
            AudioManager.Instance?.PlayJump();
        }
    }
    private bool IsOnInvalidSlopeForJump()
    {
        Vector3 origin = groundCheck != null
            ? groundCheck.position + Vector3.up * 0.15f
            : transform.position + Vector3.up * 0.15f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 0.8f, groundMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            return angle > config.maxAngleMovement;
        }

        return false;
    }
    private void UpdateSlopeState()
    {
        isOnSteepSlope = false;
        steepSlopeDownDir = Vector3.zero;

        Vector3 origin = groundCheck != null ? groundCheck.position + Vector3.up * 0.2f : transform.position + Vector3.up * 0.2f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, slopeCheckDistance, groundMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            if (angle > config.maxAngleMovement)
            {
                isOnSteepSlope = true;
                steepSlopeDownDir = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
            }
        }
    }

    private bool CanMove(Vector3 moveDir)
    {
        if (moveDir.sqrMagnitude < 0.0001f) return true;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        float distance = 0.45f;
        float radius = 0.25f;

        if (Physics.SphereCast(origin, radius, moveDir.normalized, out RaycastHit hit, distance, groundMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            if (angle > config.maxAngleMovement)
                return false;
        }

        return true;
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
        if (config == null)
            config = template.config;
        groundMask = template.groundMask;

        stepDistanceWalk = template.stepDistanceWalk;
        stepDistanceSprint = template.stepDistanceSprint;
        minSpeedForSteps = template.minSpeedForSteps;

        sprintKey = template.sprintKey;
        clips = new List<AudioClip>(template.clips);

        EnsureFootstepAudioSource();
        if (config == null)
            Debug.LogError("PlayerController2: No se pudo asignar PlayerControllerConfigSO desde el template.");
    }

    public void InjectRuntimeRefs(Transform pivotRef, Transform camRef, Transform groundCheckRef)
    {
        pivot = pivotRef;
        cameraTransform = camRef;
        firstPersonCameraTransform = camRef;
        thirdPersonCameraTransform = camRef;
        groundCheck = groundCheckRef;
    }

    private bool TryGetTerrainUnderfoot(out Terrain terrain, out Vector3 hitPoint)
    {
        terrain = null;
        hitPoint = default;

        Vector3 origin = groundCheck != null
            ? groundCheck.position + Vector3.up * 0.5f
            : transform.position + Vector3.up * 0.5f;

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, config.groundRayDistance, groundMask))
        {
            hitPoint = hit.point;

            TerrainCollider terrainCol = hit.collider as TerrainCollider;
            if (terrainCol != null)
                terrain = terrainCol.GetComponent<Terrain>();
            else
                terrain = hit.collider.GetComponent<Terrain>();

            return terrain != null;
        }

        return false;
    }
    private int GetDominantTerrainTextureIndex(Terrain terrain, Vector3 worldPos)
    {
        TerrainData data = terrain.terrainData;

        Vector3 local = worldPos - terrain.transform.position;
        float normX = Mathf.Clamp01(local.x / data.size.x);
        float normZ = Mathf.Clamp01(local.z / data.size.z);
        int x = Mathf.Clamp(Mathf.FloorToInt(normX * data.alphamapWidth), 0, data.alphamapWidth - 1);
        int z = Mathf.Clamp(Mathf.FloorToInt(normZ * data.alphamapHeight), 0, data.alphamapHeight - 1);

        float[,,] splat = data.GetAlphamaps(x, z, 1, 1);

        int best = 0;
        float bestVal = splat[0, 0, 0];

        for (int i = 1; i < data.alphamapLayers; i++)
        {
            float v = splat[0, 0, i];
            if (v > bestVal)
            {
                bestVal = v;
                best = i;
            }
        }

        return best;
    }

    private void EnsureFootstepAudioSource()
    {
        if (audioSourceSteps == null)
            audioSourceSteps = GetComponent<AudioSource>();

        if (audioSourceSteps == null)
            audioSourceSteps = gameObject.AddComponent<AudioSource>();

        audioSourceSteps.playOnAwake = false;
        audioSourceSteps.loop = false;
        audioSourceSteps.spatialBlend = 0f;
    }
}
