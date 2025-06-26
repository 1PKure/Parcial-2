using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private GameObject door;
    public float requiredMass = 3f;
    [Header("Feedback Visual")]
    [SerializeField] private Renderer plateRenderer; 
    [SerializeField] private Color colorActivated = Color.green;
    [SerializeField] private Color colorDeactivated = Color.red;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rb.mass >= requiredMass)
        {
            UIManager.Instance.ShowMessage("Plataforma activada");
            door.SetActive(false);
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rb.mass >= requiredMass)
        {
            UIManager.Instance.ShowMessage("Plataforma desactivada");
            door.SetActive(true);
        }
    }

    private void CambiarColor(bool activado)
    {
        if (plateRenderer != null)
        {
            plateRenderer.material.color = activado ? colorActivated : colorDeactivated;
        }
    }
}
