using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float possessionRange = 5f;

    private GameObject currentBody;
    private PlayerController2 originalController;
    private PlayerController2 playerController;

    private Transform cameraHolder;
    private PlayerHealth playerHealth;
    private Transform originalCameraParent;
    private Vector3 originalCamLocalPos;
    private Quaternion originalCamLocalRot;

    private Transform possessPivot;
    private Transform possessGroundCheck;

    private bool isPossessing = false;
    public bool IsPossessing => isPossessing;
    public Transform CurrentBodyTransform => currentBody != null ? currentBody.transform : null;
    private void Start()
    {
        playerHealth = GetComponent<PlayerHealth>();
        originalController = GetComponent<PlayerController2>();
        playerController = originalController;

        cameraHolder = playerController.cameraTransform;

        originalCameraParent = cameraHolder.parent;
        originalCamLocalPos = cameraHolder.localPosition;
        originalCamLocalRot = cameraHolder.localRotation;
    }
    public Transform CurrentControlledTransform
    {
        get
        {
            if (isPossessing && currentBody != null)
                return currentBody.transform;

            return playerController != null ? playerController.transform : transform;
        }
    }
    private void Update()
    {
        if (!isPossessing && Input.GetKeyDown(KeyCode.E))
            TryPossess();
        else if (isPossessing && Input.GetKeyDown(KeyCode.Q))
            Release();
    }

    void TryPossess()
    {
        Camera cam = cameraHolder.GetComponentInChildren<Camera>();
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, possessionRange))
        {
            if (hit.collider.CompareTag("Possessable"))
                Possess(hit.collider.gameObject);
        }
    }

    void Possess(GameObject target)
    {
        playerHealth?.SetInvulnerable(true);
        isPossessing = true;
        currentBody = target;
        originalController.IsPossessed = true;

        foreach (var comp in originalController.GetComponents<MonoBehaviour>())
        {
            if (comp == this) continue;                 
            if (comp is PlayerHealth) continue;
            comp.enabled = false;
        }

        possessPivot = new GameObject("PossessPivot").transform;
        possessPivot.SetParent(currentBody.transform);

        Vector3 pivotPos = currentBody.transform.position;
        var col = currentBody.GetComponent<Collider>();
        if (col != null) pivotPos = col.bounds.center;

        possessPivot.position = pivotPos;
        possessPivot.rotation = Quaternion.identity;

        cameraHolder.SetParent(possessPivot);
        cameraHolder.localPosition = Vector3.zero;
        cameraHolder.localRotation = Quaternion.identity;

        possessGroundCheck = new GameObject("PossessGroundCheck").transform;
        possessGroundCheck.SetParent(currentBody.transform);
        if (col != null)
            possessGroundCheck.position = col.bounds.min + Vector3.up * 0.1f;
        else
            possessGroundCheck.localPosition = Vector3.zero;

        PlayerController2 newController = currentBody.AddComponent<PlayerController2>();
        newController.SetupFromTemplate(originalController);
        newController.InjectRuntimeRefs(possessPivot, cameraHolder, possessGroundCheck);

        playerController = newController;
        playerController.Initialize();
        playerController.IsPossessed = true;
        playerController.ResetRotation();
    }

    void Release()
    {
        playerHealth?.SetInvulnerable(false);

        if (playerController != null && playerController != originalController)
            Destroy(playerController);

        if (possessPivot != null) Destroy(possessPivot.gameObject);
        if (possessGroundCheck != null) Destroy(possessGroundCheck.gameObject);

        currentBody = null;
        isPossessing = false;
        originalController.IsPossessed = false;
        playerController = originalController;

        foreach (var comp in originalController.GetComponents<MonoBehaviour>())
            comp.enabled = true;

        cameraHolder.SetParent(originalCameraParent);
        cameraHolder.localPosition = originalCamLocalPos;
        cameraHolder.localRotation = originalCamLocalRot;

        playerController.cameraTransform = cameraHolder;
        playerController.ResetRotation();

        var cam = cameraHolder.GetComponentInChildren<Camera>(true);
        if (cam != null)
        {
            cam.gameObject.SetActive(true);
            cam.enabled = true;
        }

        var listener = cameraHolder.GetComponentInChildren<AudioListener>(true);
        if (listener != null)
        {
            listener.gameObject.SetActive(true);
            listener.enabled = true;
        }

        var camController = originalController.GetComponent<CameraController>();
        if (camController != null)
            camController.ForceFirstPerson();
    }
}
