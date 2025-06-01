using UnityEngine;

public class WispStateChanger : MonoBehaviour
{
    private Renderer rend;
    public Vector2 baseOffset;
    public Vector2 possessedOffset;

    public void SetBaseState()
    {
        rend.material.mainTextureOffset = baseOffset;
    }

    public void SetPossessedState()
    {
        rend.material.mainTextureOffset = possessedOffset;
    }

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    public void OnPossessed()
    {
        if (rend != null)
        {
            rend.material.color = Color.gray;
        }

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = false;
        }
    }

    public void OnReleased()
    {
        if (rend != null)
        {
            rend.material.color = Color.white;
        }

        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}
