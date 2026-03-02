using UnityEngine;

public class EnemyTargeter : MonoBehaviour
{
    public Transform PlayerTarget { get; private set; }

    private void OnEnable()
    {

        PlayerTarget = PlayerRegistry.Player;

        PlayerRegistry.OnPlayerRegistered += HandlePlayerRegistered;
    }

    private void OnDisable()
    {
        PlayerRegistry.OnPlayerRegistered -= HandlePlayerRegistered;
    }

    private void HandlePlayerRegistered(Transform t)
    {
        PlayerTarget = t;
    }
}