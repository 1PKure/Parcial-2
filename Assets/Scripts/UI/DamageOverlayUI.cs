using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DamageOverlayUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image overlay;

    [Header("Fade")]
    [SerializeField, Range(0f, 1f)] private float maxAlpha = 0.45f;
    [SerializeField] private float fadeInTime = 0.05f;
    [SerializeField] private float fadeOutTime = 0.25f;

    [Header("Player")]
    [SerializeField] private PlayerHealth playerHealth;

    private Coroutine routine;

    private void Reset()
    {
        overlay = GetComponent<Image>();
    }

    private void Awake()
    {
        if (overlay != null)
        {
            var c = overlay.color;
            overlay.color = new Color(c.r, c.g, c.b, 0f);
        }

        if (playerHealth == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerHealth = player.GetComponent<PlayerHealth>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth != null)
            playerHealth.OnDamaged += HandleDamaged;
    }

    private void OnDisable()
    {
        if (playerHealth != null)
            playerHealth.OnDamaged -= HandleDamaged;
    }

    private void HandleDamaged(float amount)
    {
        PlayHit();
    }

    [ContextMenu("TEST/Play Hit")]
    private void TestPlayHit()
    {
        PlayHit();
    }

    public void PlayHit()
    {
        if (overlay == null) return;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(HitRoutine());
    }

    private IEnumerator HitRoutine()
    {
        yield return FadeTo(maxAlpha, fadeInTime);
        yield return FadeTo(0f, fadeOutTime);
        routine = null;
    }

    private IEnumerator FadeTo(float targetAlpha, float time)
    {
        Color c = overlay.color;
        float startAlpha = c.a;

        if (time <= 0f)
        {
            overlay.color = new Color(c.r, c.g, c.b, targetAlpha);
            yield break;
        }

        float t = 0f;
        while (t < time)
        {
            t += Time.unscaledDeltaTime;
            float a = Mathf.Lerp(startAlpha, targetAlpha, t / time);
            overlay.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }

        overlay.color = new Color(c.r, c.g, c.b, targetAlpha);
    }
}