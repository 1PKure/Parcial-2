using UnityEngine;
using UnityEngine.UI;

public class StaminaBarUI : MonoBehaviour
{
    [SerializeField] private PlayerController2 player;
    [SerializeField] private Image fillImage;

    [Header("Auto Find")]
    [SerializeField] private bool autoFindPlayer = true;
    [SerializeField] private float retryEvery = 0.25f;
    private float nextRetry;

    [Header("Smoothing")]
    [SerializeField] private bool smooth = true;
    [SerializeField] private float smoothSpeed = 12f;

    private float currentFill = 1f;
    private float targetFill = 1f;

    private void Awake()
    {
        if (fillImage == null)
            fillImage = GetComponent<Image>();
    }

    private void OnEnable()
    {
        TryBind(forceApply: true);
    }

    private void OnDisable()
    {
        Unbind();
    }

    private void Update()
    {
        if (player == null && autoFindPlayer && Time.unscaledTime >= nextRetry)
        {
            nextRetry = Time.unscaledTime + retryEvery;
            TryBind(forceApply: true);
        }

        if (!smooth || fillImage == null) return;

        currentFill = Mathf.Lerp(currentFill, targetFill, Time.unscaledDeltaTime * smoothSpeed);
        Apply(currentFill);
    }

    private void TryBind(bool forceApply)
    {
        if (player == null && autoFindPlayer)
            player = FindFirstObjectByType<PlayerController2>();

        if (player == null) return;

        player.OnStaminaNormalizedChanged -= HandleStaminaChanged;
        player.OnStaminaNormalizedChanged += HandleStaminaChanged;

        targetFill = player.StaminaNormalized;
        if (forceApply)
        {
            currentFill = targetFill;
            Apply(currentFill);
        }
    }

    private void Unbind()
    {
        if (player != null)
            player.OnStaminaNormalizedChanged -= HandleStaminaChanged;
    }

    private void HandleStaminaChanged(float normalized)
    {
        targetFill = Mathf.Clamp01(normalized);

        if (!smooth)
        {
            currentFill = targetFill;
            Apply(currentFill);
        }
    }

    private void Apply(float value)
    {
        fillImage.fillAmount = Mathf.Clamp01(value);
    }
}