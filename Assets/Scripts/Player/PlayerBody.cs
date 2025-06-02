using UnityEngine;

public class PlayerBody : MonoBehaviour, IPossessable
{
    private Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void OnPossessed()
    {
        if (rend != null)
            rend.material.color = Color.gray;
        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    public void OnReleased()
    {
        if (rend != null)
            rend.material.color = Color.white;
        var playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}
