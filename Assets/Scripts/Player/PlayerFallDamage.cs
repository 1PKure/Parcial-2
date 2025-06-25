using UnityEngine;

[RequireComponent(typeof(PlayerHealth))]
public class PlayerFallDamage : MonoBehaviour
{
    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundDistance = 0.4f;

    [Header("Caída")]
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private float fallDamage = 20f;

    private float lastYPosition;
    private bool wasGrounded = true;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (!isGrounded && wasGrounded)
        {
            lastYPosition = transform.position.y;
        }

        if (isGrounded && !wasGrounded)
        {
            float fallDistance = lastYPosition - transform.position.y;

            if (fallDistance > Mathf.Abs(fallThreshold))
            {

                UIManager.Instance.ShowMessage($"Daño por caída: {fallDamage}");
                playerHealth.TakeDamage(fallDamage);
            }
        }

        wasGrounded = isGrounded;
    }
}
