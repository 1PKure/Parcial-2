using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera firstPersonCamera;
    [SerializeField] private Camera thirdPersonCamera;
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 thirdPersonOffset = new Vector3(0, 2, -5);

    private bool isFirstPerson = true;
    private float transitionSpeed = 1.5f;
    public Transform target;

    void Start()
    {
        ForceFirstPerson();
        SetTarget(player);
    }

    void Update()
    {

        /*
        if (Input.GetKeyDown(KeyCode.C))
        {
            isFirstPerson = !isFirstPerson;
            SetCameraMode(isFirstPerson);
        }
        */

        if (!isFirstPerson && thirdPersonCamera.gameObject.activeSelf && target != null)
        {
            Vector3 targetPosition = target.position + target.TransformDirection(thirdPersonOffset);
            thirdPersonCamera.transform.position = Vector3.Lerp(
                thirdPersonCamera.transform.position,
                targetPosition,
                Time.deltaTime * transitionSpeed);

            thirdPersonCamera.transform.LookAt(target);
        }
    }

    void SetCameraMode(bool firstPerson)
    {
        firstPersonCamera.gameObject.SetActive(firstPerson);
        //thirdPersonCamera.gameObject.SetActive(!firstPerson);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ForceFirstPerson()
    {
        isFirstPerson = true;

        if (firstPersonCamera != null)
            firstPersonCamera.gameObject.SetActive(true);

        if (thirdPersonCamera != null)
            thirdPersonCamera.gameObject.SetActive(false);
    }
}