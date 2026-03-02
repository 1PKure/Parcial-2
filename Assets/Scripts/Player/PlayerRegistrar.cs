using UnityEngine;

public class PlayerRegistrar : MonoBehaviour
{
    private void OnEnable() => PlayerRegistry.Register(transform);
    private void OnDisable() => PlayerRegistry.Unregister(transform);
}