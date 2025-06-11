using Clase10;
using UnityEngine;

public class GhostController : MonoBehaviour
{
    [SerializeField] private float possessionRange = 5f;
    private GameObject currentBody;
    [SerializeField] private PlayerController playerController;
    private Transform originalBody;
    private bool isPossessing = false;

    private void Start()
    {
        originalBody = GameObject.FindWithTag("Player").transform;
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        originalBody = playerController.transform;

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
        //playerController.ChangeState(new PlayerPossessedState(playerController));
        //playerController.SetCameraTarget(currentBody.transform);
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

            playerController.SetCameraTarget(currentBody.transform);
            //playerController.ChangeState(new PlayerIdleState(playerController));
        }
    }
}
