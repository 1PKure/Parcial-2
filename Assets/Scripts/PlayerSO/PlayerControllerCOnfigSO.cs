using UnityEngine;

[CreateAssetMenu(menuName = "Game/Player/Player Controller Config", fileName = "PlayerControllerConfig")]
public class PlayerControllerConfigSO : ScriptableObject
{
    [Header("Movement")]
    public float maxAngleMovement = 30f;
    public float moveSpeed = 5f;

    [Header("Jump")]
    public float jumpCooldown = 0.3f;
    public int maxJumpCount = 1;
    public float jumpHeight = 5f;
    public float gravity = -9.81f;
    public float groundDistance = 0.4f;
    public float groundRayDistance = 3f;
    public float groundCheckRadius = 0.2f;

    [Header("Rotation / Camera")]
    public float mouseSensitivity = 200f;
    public float maxLookAngle = 80f;

    [Header("Stamina / Sprint")]
    public bool enableStamina = true;
    public float maxStamina = 100f;
    public float staminaDrainPerSecond = 25f;
    public float staminaRegenPerSecond = 20f;
    public float staminaRegenDelay = 0.75f;
    public float minStaminaToStartSprint = 10f;
    public float sprintMultiplier = 1.6f;
}