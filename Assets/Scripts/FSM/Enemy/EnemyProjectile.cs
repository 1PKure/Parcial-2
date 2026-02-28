using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 10;
    public float lifetime = 5f;

    private Rigidbody rb;

    [Header("Audio 3D")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip shootClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private Vector2 pitchRange = new Vector2(0.95f, 1.05f);

    private Vector3 moveDir;
    private bool initialized;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake = false;
            sfxSource.spatialBlend = 1f;      
            sfxSource.rolloffMode = AudioRolloffMode.Logarithmic;
            sfxSource.minDistance = 2.5f;
            sfxSource.maxDistance = 25f;
        }
    }
    private void Start()
    {
        if (!initialized)
        {
            Init(transform.forward, damage, speed);
        }
    }
    public void Init(Vector3 direction, int damage, float speed)
    {
        moveDir = direction.normalized;
        this.damage = damage;
        this.speed = speed;

        initialized = true;

        if (moveDir.sqrMagnitude > 0.0001f)
            transform.forward = moveDir;

        if (rb != null)
            rb.velocity = moveDir * this.speed;

        if (shootClip != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            sfxSource.PlayOneShot(shootClip);
        }

        CancelInvoke();
        Invoke(nameof(SelfDestruct), lifetime);
    }

    private void SelfDestruct()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var health = other.GetComponent<PlayerHealth>();
            if (health != null)
                health.TakeDamage(damage);
        }

        if (hitClip != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            sfxSource.PlayOneShot(hitClip);
            Destroy(gameObject, 0.5f); 
        }

        Destroy(gameObject);
    }
}
