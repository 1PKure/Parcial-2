using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float possessionRange = 5f;
    private GameObject currentBody;
    [SerializeField] private PlayerController2 playerController;
    private Transform originalBody;
    private Transform cameraHolder;
    private CameraController cameraController;
    private bool isPossessing = false;

    private void Start()
    {
        originalBody = GameObject.FindWithTag("Player").transform;
        playerController = originalBody.GetComponent<PlayerController2>();
        cameraHolder = playerController.cameraTransform;
        cameraController = cameraHolder.GetComponent<CameraController>();
    }

    private void Update()
    {
        if (!isPossessing && Input.GetKeyDown(KeyCode.E))
        {
            TryPossess();
        }
        else if (isPossessing && Input.GetKeyDown(KeyCode.Q))
        {
            Release();
        }
    }

    void TryPossess()
    {
        // Obtenemos la cámara del holder, que puede estar en un hijo
        Camera cam = cameraHolder != null ? cameraHolder.GetComponentInChildren<Camera>() : null;

        if (cam == null)
        {
            Debug.LogWarning("Cámara no encontrada al intentar poseer.");
            return;
        }

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        Debug.DrawRay(ray.origin, ray.direction * possessionRange, Color.red, 1f);

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
        playerController.IsPossessed = true;
        playerController.enabled = false;

        foreach (var comp in originalBody.GetComponents<MonoBehaviour>())
        {
            if (comp != this)
                comp.enabled = false;
        }

        currentBody = target;

        if (!currentBody.TryGetComponent(out PlayerPossessedController _))
        {
            currentBody.AddComponent<PlayerPossessedController>();
        }

        cameraHolder.SetParent(currentBody.transform);
        cameraHolder.localPosition = Vector3.zero;
        cameraHolder.localRotation = Quaternion.identity;

        cameraController.SetTarget(currentBody.transform);
        playerController.SetCameraTarget(currentBody.transform);
    }

    void Release()
    {
        if (currentBody != null)
        {
            var possessedController = currentBody.GetComponent<PlayerPossessedController>();
            if (possessedController != null)
                Destroy(possessedController);

            currentBody = null;
        }

        isPossessing = false;
        playerController.IsPossessed = false;

        foreach (var comp in originalBody.GetComponents<MonoBehaviour>())
        {
            comp.enabled = true;
        }

        cameraHolder.SetParent(originalBody);
        cameraHolder.localPosition = Vector3.zero;
        cameraHolder.localRotation = Quaternion.identity;

        cameraController.SetTarget(originalBody);
        playerController.SetCameraTarget(originalBody);
        playerController.ResetRotation();
    }
}
