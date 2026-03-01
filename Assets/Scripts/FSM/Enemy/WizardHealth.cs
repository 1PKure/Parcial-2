using System;
using UnityEngine;

public class WizardHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;

    public float CurrentHealth { get; private set; }
    public bool IsDead { get; private set; }

    public event Action<WizardHealth> OnDied;

    private WizardEnemyController wizardAnim;
    private RangedEnemyController ai;
    private Collider[] colliders;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        wizardAnim = GetComponent<WizardEnemyController>();
        ai = GetComponent<RangedEnemyController>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;

        CurrentHealth -= amount;
        if (CurrentHealth > 0f) return;

        Die();
    }

    private void Die()
    {
        IsDead = true;
        CurrentHealth = 0f;

        if (ai != null) ai.enabled = false;

        wizardAnim?.PlayDieAnimation();

        foreach (var c in colliders)
            c.enabled = false;

        OnDied?.Invoke(this);

        Destroy(gameObject, 3.5f);
    }
}