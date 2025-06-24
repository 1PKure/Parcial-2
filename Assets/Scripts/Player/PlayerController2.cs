using System.Collections.Generic;
using UnityEngine;


public class PlayerController2 : MonoBehaviour
{
    public Transform pivot;
    public Transform cameraTransform;

    [Header("Movement")]
    [SerializeField] private float maxAngleMovement = 30f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> clips = new List<AudioClip>();
    [SerializeField] float maxTimeAudio = 0.7f;
    [SerializeField] float timeAudio = 0;

    [Header("Camera")]
    [SerializeField] private Transform firstPersonCameraTransform;
    [SerializeField] private Transform thirdPersonCameraTransform;

    private bool isFirstPerson = true;
    private bool isGrounded;
    private Rigidbody rb;
    private bool isMoving = false;
    private float jumpHeight = 2f;
    private float gravity = -9.81f;
    private float groundDistance = 0.4f;
    // FSM
    private StateMachine stateMachine;
    private float mouseSensitivity = 100f;
    private float rotationY = 0f;
    private float rotationX = 0f;
    private float maxAngle = 30f;
    private bool JumpPressed => Input.GetKeyDown(KeyCode.Space);
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        stateMachine = new StateMachine();
        stateMachine.AddState(new PlayerIdleState(this));
        stateMachine.AddState(new PlayerMoveState(this));
        stateMachine.AddState(new PlayerPossessedState(this));
        stateMachine.ChangeState(StateType.Idle);
        cameraTransform = isFirstPerson ? firstPersonCameraTransform : thirdPersonCameraTransform;
    }

    private void Update()
    {
        stateMachine.Update();
        HandleJump();
        HandleRotation();
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C key pressed, toggling camera view.");
            isFirstPerson = !isFirstPerson;

            cameraTransform = isFirstPerson ? firstPersonCameraTransform : thirdPersonCameraTransform;
        }
    }

    public void Move(Vector3 moveDir)
    {
        Vector3 velocity = new Vector3(moveDir.x * moveSpeed, rb.velocity.y, moveDir.z * moveSpeed);

        if (CanMove(moveDir))
        {
            PlayAudio();
            rb.velocity = velocity;
        }
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
        }

        if (JumpPressed && isGrounded)
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * -2f * gravity);
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        }

        rb.velocity += new Vector3(0, gravity * Time.deltaTime, 0);
    }


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

    private void PlayAudio()
    {
        timeAudio += Time.deltaTime;

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

        if (timeAudio > maxTimeAudio)
        {
            timeAudio = 0;
            if (index < clips.Count)
                audioSource.clip = clips[index];
            //audioSource.Play();
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

    // Métodos de la FSM
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
}
