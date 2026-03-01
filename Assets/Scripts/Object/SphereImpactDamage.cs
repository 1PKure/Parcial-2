using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereImpactDamage : MonoBehaviour
{
    [Header("Damage Tuning")]
    [SerializeField] private float damageFactor = 10f;     
    [SerializeField] private float minDamage = 15f;        
    [SerializeField] private float minSpeedToDamage = 2.5f;

    private Rigidbody rb;
    private PlayerController2 controller;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controller = GetComponent<PlayerController2>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (controller == null || !controller.IsPossessed) return;

        var wizardHealth = collision.collider.GetComponentInParent<WizardHealth>();
        if (wizardHealth == null || wizardHealth.IsDead) return;

        float speed = rb.velocity.magnitude;
        if (speed < minSpeedToDamage) return;

        float dmg = Mathf.Max(minDamage, speed * damageFactor);
        wizardHealth.TakeDamage(dmg);
    }
}