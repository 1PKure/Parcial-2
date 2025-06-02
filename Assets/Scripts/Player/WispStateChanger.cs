using UnityEngine;

public class WispStateChanger : MonoBehaviour
{
    private Renderer rend;
    public Vector2 baseOffset;
    public Vector2 possessedOffset;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }
    public void SetBaseState()
    {
        rend.material.mainTextureOffset = baseOffset;
    }

    public void SetPossessedState()
    {
        rend.material.mainTextureOffset = possessedOffset;
    }
}
