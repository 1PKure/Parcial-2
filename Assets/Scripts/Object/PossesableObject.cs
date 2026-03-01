using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PossessableObject : Person, IPossessable

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

    public override void Initialize()
    {
        Debug.Log("Inicializado");
    }

    public override void EnableControl()
    {

        enabled = true;
    }

    public override void DisableControl()
    {
        enabled = false;
    }

    public override Transform GetCameraTarget()
    {
        return transform;
    }
}
