using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[SerializeField] private PlayerData playerData;
    [SerializeField] private Transform cameraHolder;
    public Transform cameraTransform;

    private CharacterController controller;
    private Animator animator;
    private float xRotation = 0f;
    private int currentHealth;
    private Camera activeCamera;
    private float mouseX = 0f;
    private float mouseY = 0f;
    private float moveSpeed = 5f;
    private Vector3 velocity;
    private bool isGrounded;
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        ResetPlayer();
        UpdateActiveCamera();
    }

    public void ResetPlayer()
    {
        //transform.position = playerData.spawnPosition;
        //currentHealth = playerData.maxHealth;
        xRotation = 0f;
    }

    void Update()
    {
        HandleMovement();
        UpdateActiveCamera();
        MouseRotation();
    }

    void HandleMovement()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Jump") && isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        Vector3 move = transform.right * x + transform.forward * z;
        move *= moveSpeed * Time.deltaTime;
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        controller.Move(move);
    }

    private void UpdateActiveCamera()
    {
        activeCamera = Camera.main;
    }
    private float sensitivityMultiplier;
    private float mouseSensitivity = 10f;

    private void MouseRotation()
    {
        float mouseXInput = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime * 100;
        float mouseYInput = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime * 100;

        mouseX += mouseXInput;
        mouseY -= mouseYInput;
        mouseY = Mathf.Clamp(mouseY, -80f, 80f);

        cameraHolder.transform.localRotation = Quaternion.Euler(mouseY, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, mouseX, 0f);
    }

    public void SetCameraTarget(Transform newTarget)
    {
        cameraTransform = newTarget;
    }
}
