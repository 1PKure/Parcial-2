using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AdditiveInteriorPortal : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string interiorSceneName = "Interior_Cave";
    [SerializeField] private bool isEntrance = true;

    [Header("Interact")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;
    [SerializeField] private GameObject instructionText;

    //public GameObject World;
    private bool _playerInside;
    private bool _busy;
    private Transform _player;

    private Transform _cachedInteriorSpawn;
    private Transform _cachedExteriorSpawn;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;
        _player = other.transform;
        if (instructionText != null) instructionText.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (_busy) return;

        _playerInside = false;
        _player = null;
        if (instructionText != null) instructionText.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInside || _busy) return;
        if (Input.GetKeyDown(interactKey))
            StartCoroutine(CoUsePortal());
    }

    private IEnumerator CoUsePortal()
    {
        _busy = true;
        Time.timeScale = 1f;
        if (instructionText != null) instructionText.SetActive(false);
        var playerTransform = _player;

        if (_cachedExteriorSpawn == null)
            _cachedExteriorSpawn = FindSpawnInActiveScenes(SpawnId.Exterior);

        if (isEntrance)
        {
            
            //World.SetActive(false);
            SceneLoader.Instance.LoadSceneAdditive(interiorSceneName);

            while (!SceneManager.GetSceneByName(interiorSceneName).isLoaded)
                yield return null;

            yield return null; 

            if (_cachedInteriorSpawn == null)
                _cachedInteriorSpawn = FindSpawnInScene(interiorSceneName, SpawnId.Interior);

            Teleport(playerTransform, _cachedInteriorSpawn);
        }
        else
        {
            //World.SetActive(true);

            Teleport(playerTransform, _cachedExteriorSpawn);

            SceneLoader.Instance.UnloadScene(interiorSceneName);
            while (SceneManager.GetSceneByName(interiorSceneName).isLoaded)
                yield return null;
        }

        _busy = false;
    }

    private void Teleport(Transform playerTransform, Transform target)
    {
        if (playerTransform == null) { Debug.LogError("Player null."); return; }
        if (target == null) { Debug.LogError("Spawn target null."); return; }

        playerTransform.position = target.position;
        playerTransform.rotation = target.rotation;
    }

    private Transform FindSpawnInScene(string sceneName, SpawnId id)
    {
        Scene s = SceneManager.GetSceneByName(sceneName);
        if (!s.isLoaded)
        {
            Debug.LogError($" Scene '{sceneName}' no está cargada.");
            return null;
        }

        foreach (var root in s.GetRootGameObjects())
        {
            var markers = root.GetComponentsInChildren<SpawnPointMarker>(true);
            foreach (var m in markers)
                if (m.id == id) return m.transform;
        }

        return null;
    }

    private Transform FindSpawnInActiveScenes(SpawnId id)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene s = SceneManager.GetSceneAt(i);
            foreach (var root in s.GetRootGameObjects())
            {
                var markers = root.GetComponentsInChildren<SpawnPointMarker>(true);
                foreach (var m in markers)
                    if (m.id == id) return m.transform;
            }
        }
        return null;
    }
}
