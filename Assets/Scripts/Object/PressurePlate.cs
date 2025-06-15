using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public GameObject puerta;
    public float requiredMass = 3f;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rb.mass >= requiredMass)
        {
            Debug.Log("Plataforma activada");
            puerta.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && rb.mass >= requiredMass)
        {
            Debug.Log("Plataforma desactivada");
            puerta.SetActive(true);
        }
    }
}
