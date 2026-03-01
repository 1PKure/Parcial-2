using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereImpactDamage : MonoBehaviour
{
    [Header("Damage Tuning")]
    [SerializeField] private float damageFactor = 10f;     
    [SerializeField] private float minDamage = 15f;        
    [SerializeField] private float minSpeedToDamage = 2.5f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        var controller = GetComponent<PlayerController2>();
        if (controller == null || !controller.IsPossessed) return;

        var wizardHealth = collision.collider.GetComponentInParent<WizardHealth>();
        if (wizardHealth == null || wizardHealth.IsDead) return;

        float speed = rb.velocity.magnitude;
        if (speed < minSpeedToDamage) return;

        float dmg = Mathf.Max(minDamage, speed * damageFactor);
        wizardHealth.TakeDamage(dmg);
    }
}