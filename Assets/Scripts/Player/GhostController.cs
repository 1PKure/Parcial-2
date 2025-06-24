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
    private StateMachine stateMachine;


    private void Start()
    {
        originalBody = GameObject.FindWithTag("Player").transform;
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController2>();
        originalBody = playerController.transform;
        cameraHolder = playerController.cameraTransform;
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
        isPossessing = true;

        IPossessable possessable = target.GetComponent<IPossessable>();
        if (possessable == null) return;

        if (originalBody.TryGetComponent(out IPossessable originalPossessable))
            originalPossessable.OnPossessed();

        playerController.enabled = false;
        foreach (var component in originalBody.GetComponents<MonoBehaviour>())
        {
            if (component != this)
                component.enabled = false;
        }

        currentBody = target;

        if (currentBody.TryGetComponent(out IPossessable newPossessed))
            newPossessed.OnPossessed();

        currentBody.AddComponent<PlayerPossessedController>();


        cameraHolder.SetParent(currentBody.transform);
        cameraHolder.localPosition = Vector3.zero;
        cameraHolder.localRotation = Quaternion.identity;

 
        cameraController.SetTarget(currentBody.transform);
        playerController.SetCameraTarget(currentBody.transform);


        playerController.ChangeState(StateType.Possessed);
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

            playerController.enabled = true;
            foreach (var component in originalBody.GetComponents<MonoBehaviour>())
            {
                component.enabled = true;
            }

            cameraHolder.SetParent(originalBody);
            cameraHolder.localPosition = Vector3.zero;
            cameraHolder.localRotation = Quaternion.identity;

            playerController.SetCameraTarget(originalBody);
            cameraController.SetTarget(originalBody);


            playerController.ChangeState(StateType.Idle);
        }
    }

}
