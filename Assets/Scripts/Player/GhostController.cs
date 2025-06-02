using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float possessionRange = 5f;
    private GameObject currentBody;
    [SerializeField] private CameraController cameraController;
    private Transform originalBody;
    private bool isPossessing = false;

    private void Start()
    {
        originalBody = GameObject.FindWithTag("Player").transform;
        if (cameraController == null)
            Debug.LogError("CameraController no asignado en GhostController!");

        if (cameraController != null)
            cameraController.SetTarget(originalBody);

    }
    void Update()
    {
        if (!isPossessing && Input.GetKeyDown(KeyCode.E))
        {
            TryPossess();
        }
        else if (isPossessing && Input.GetKeyDown(KeyCode.Q) && currentBody != null)
        {
            Release();
        }
    }

    void TryPossess()
    {

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
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
        isPossessing = true;
        IPossessable possessable = target.GetComponent<IPossessable>();
        if (possessable == null) return;

        if (originalBody.TryGetComponent(out IPossessable originalPossessable))
            originalPossessable.OnPossessed();

        currentBody = target;

        if (currentBody.TryGetComponent(out IPossessable newPossessed))
            newPossessed.OnPossessed();

        currentBody.AddComponent<PlayerPossessedController>();
        cameraController.SetTarget(currentBody.transform);
    }

    void Release()
    {

        if (currentBody != null)
        {
            if (currentBody.TryGetComponent(out IPossessable possessed))
                possessed.OnReleased();

            var possessedController = currentBody.GetComponent<PlayerPossessedController>();
            if (possessedController != null)
                Destroy(possessedController);

            currentBody = null;
            isPossessing = false;


            if (originalBody.TryGetComponent(out IPossessable originalPossessable))
                originalPossessable.OnReleased();

            cameraController.SetTarget(originalBody);
        }
    }
}
