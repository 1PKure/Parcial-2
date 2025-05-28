using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PossessableObject : MonoBehaviour, IPossessable
{
    private Renderer rend;

    private void Awake()
    {
        tag = "Possessable";
        rend = GetComponent<Renderer>();
    }

    public void OnPossessed()
    {
        
        if (rend != null)
            rend.material.color = Color.green;
    }

    public void OnReleased()
    {
        if (rend != null)
            rend.material.color = Color.white;
    }
}
