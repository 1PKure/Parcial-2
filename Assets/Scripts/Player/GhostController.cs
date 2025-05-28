using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float possessionRange = 5f;
    private GameObject currentBody;
    private CameraController cameraController;
    private Transform originalBody;

    private void Start()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        originalBody = transform;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryPossess();
        }
        else if (Input.GetKeyDown(KeyCode.Q) && currentBody != null)
        {
            Release();
        }
    }

    void TryPossess()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, possessionRange))
        {
            if (hit.collider.CompareTag("Possessable"))
            {
                Possess(hit.collider.gameObject);
            }
        }
    }

    void Possess(GameObject target)
    {
        IPossessable possessable = target.GetComponent<IPossessable>();
        if (possessable == null) return;

        currentBody = Instantiate(target, target.transform.position, target.transform.rotation);

        if (currentBody.TryGetComponent(out IPossessable newPossessed))
            newPossessed.OnPossessed();

        currentBody.AddComponent<PlayerPossessedController>();
        cameraController.SetTarget(currentBody.transform); 

        gameObject.SetActive(false); 
    }

    void Release()
    {
        if (currentBody != null)
        {
            if (currentBody.TryGetComponent(out IPossessable possessed))
                possessed.OnReleased();

            transform.position = currentBody.transform.position;

            Destroy(currentBody);
            currentBody = null;

            gameObject.SetActive(true);
            cameraController.SetTarget(originalBody); 
        }
    }
}
