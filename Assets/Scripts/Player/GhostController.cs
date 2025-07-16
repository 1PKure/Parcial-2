using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float possessionRange = 5f;
    private GameObject currentBody;
    private PlayerController2 originalController;
    private PlayerController2 playerController;
    private Transform cameraHolder;
    private bool isPossessing = false;

    private void Start()
    {
        originalController = GetComponent<PlayerController2>();
        playerController = originalController;
        cameraHolder = playerController.cameraTransform;
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
        Camera cam = cameraHolder.GetComponentInChildren<Camera>();
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
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
        currentBody = target;

        foreach (var comp in originalController.GetComponents<MonoBehaviour>())
        {
            if (comp != this)
                comp.enabled = false;
        }

        PlayerController2 newController = currentBody.AddComponent<PlayerController2>();
        newController.SetupFromTemplate(originalController);
        playerController = newController;
        playerController.Initialize();
        playerController.IsPossessed = true;

        cameraHolder.SetParent(currentBody.transform);
        cameraHolder.localPosition = Vector3.zero;
        cameraHolder.localRotation = Quaternion.identity;

        playerController.cameraTransform = cameraHolder;
        playerController.ResetRotation();
    }

    void Release()
    {
        if (currentBody != null && playerController != null)
        {
            Destroy(playerController);
            currentBody = null;
        }

        isPossessing = false;
        playerController = originalController;

        foreach (var comp in originalController.GetComponents<MonoBehaviour>())
            comp.enabled = true;

        cameraHolder.SetParent(originalController.transform);
        cameraHolder.localPosition = Vector3.zero;
        cameraHolder.localRotation = Quaternion.identity;

        playerController.cameraTransform = cameraHolder;
        playerController.ResetRotation();
    }
}
