using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SphereDashLaunch : MonoBehaviour
{
    [Header("Launch")]
    [SerializeField] private KeyCode launchKey = KeyCode.LeftControl;

    [SerializeField] private float launchSpeed = 18f;
    [SerializeField] private float cooldown = 0.35f;

    [Header("Clamp")]
    [SerializeField] private float maxSpeed = 22f;

    [Header("Direction")]
    [SerializeField] private bool useCameraForward = true;

    private Rigidbody rb;
    private float lastTime = -999f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(launchKey))
            Launch();
    }

    private void Launch()
    {
        if (rb == null) return;

        if (rb.isKinematic)
        {
            Debug.LogError("[SphereDashLaunch] Rigidbody isKinematic = true. No se puede lanzar.");
            return;
        }

        if (Time.time - lastTime < cooldown) return;
        lastTime = Time.time;

        rb.WakeUp();

        Vector3 dir = GetLaunchDirection();
        rb.velocity = dir * launchSpeed;

        if (rb.velocity.magnitude > maxSpeed)
            rb.velocity = rb.velocity.normalized * maxSpeed;
    }

    private Vector3 GetLaunchDirection()
    {
        if (useCameraForward && Camera.main != null)
        {
            Vector3 f = Camera.main.transform.forward;
            f.y = 0f;
            if (f.sqrMagnitude > 0.001f) return f.normalized;
        }

        Vector3 t = transform.forward;
        t.y = 0f;
        return t.sqrMagnitude > 0.001f ? t.normalized : Vector3.forward;
    }
}