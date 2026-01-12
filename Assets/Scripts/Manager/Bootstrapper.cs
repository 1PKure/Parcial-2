using UnityEngine;

public class Bootstrapper : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoaderPrefab;

    private void Awake()
    {
        if (SceneLoader.Instance != null) return;

        if (sceneLoaderPrefab == null)
        {
            Debug.LogError("[Bootstrapper] No hay SceneLoader Prefab asignado.");
            return;
        }

        Instantiate(sceneLoaderPrefab);
        Debug.Log("[Bootstrapper] SceneLoader instanciado.");
    }
}
