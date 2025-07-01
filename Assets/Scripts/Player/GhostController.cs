using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float possessionRange = 5f;
    private Person currentBody;
    private Person originalBody;
    [SerializeField] private Transform cameraHolder;
    private CameraController cameraController;
    private bool isPossessing = false;
    private StateMachine stateMachine;


    private void Start()
    {
        originalBody = GameObject.FindWithTag("Player").GetComponent<Person>();
        originalBody.Initialize();
        cameraController = cameraHolder.GetComponent<CameraController>();
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
        if (!target.TryGetComponent(out Person newBody)) return;

        isPossessing = true;

        originalBody.DisableControl();

        currentBody = newBody;
        currentBody.Initialize();
        currentBody.EnableControl();

        cameraHolder.SetParent(currentBody.GetCameraTarget());
        cameraHolder.localPosition = Vector3.zero;
        cameraHolder.localRotation = Quaternion.identity;

        cameraController.SetTarget(currentBody.GetCameraTarget());
    }


    void Release()
    {
        if (currentBody != null)
        {
            currentBody.DisableControl();
            currentBody = null;
            isPossessing = false;

            originalBody.EnableControl();

            cameraHolder.SetParent(originalBody.GetCameraTarget());
            cameraHolder.localPosition = Vector3.zero;
            cameraHolder.localRotation = Quaternion.identity;

            cameraController.SetTarget(originalBody.GetCameraTarget());
        }
    }

}
