using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AdditiveInteriorPortal : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private string interiorSceneName = "Interior_Cave";
    [SerializeField] private bool isEntrance = true;

    /*
    [Header("Teleport / Positioning")]
    [SerializeField] private bool moveInteriorRoot = true;
    [SerializeField] private Vector3 interiorWorldOffset = new Vector3(0f, 0f, 250f);
    private static bool _interiorWasMoved;
    */

    [Header("Interact")]
    [SerializeField] private KeyCode interactKey = KeyCode.F; 
    [SerializeField] private GameObject instructionText;

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
        if (playerTransform == null)
        {
            Debug.LogError("[Portal] Player null.");
            _busy = false;
            yield break;
        }

        /*
        if (!isEntrance && _cachedExteriorSpawn == null)
            _cachedExteriorSpawn = FindSpawnInActiveScenes(SpawnId.Exterior);
        */

        if (isEntrance)
        {
            SceneLoader.Instance.LoadSceneAdditive(interiorSceneName);

            while (!SceneManager.GetSceneByName(interiorSceneName).isLoaded)
                yield return null;


            while (SceneLoader.Instance != null && SceneLoader.Instance.IsLoading)
                yield return null;

            /*
            if (moveInteriorRoot && !_interiorWasMoved)
            {
                MoveInteriorRoots(interiorSceneName, interiorWorldOffset);
                _interiorWasMoved = true;
            }
            */

 
            if (_cachedInteriorSpawn == null)
                _cachedInteriorSpawn = FindSpawnInScene(interiorSceneName, SpawnId.Interior);

            TeleportSafe(playerTransform, _cachedInteriorSpawn);
        }
        else
        {
            TeleportSafe(playerTransform, _cachedExteriorSpawn);

            SceneLoader.Instance.UnloadScene(interiorSceneName);
            while (SceneManager.GetSceneByName(interiorSceneName).isLoaded)
                yield return null;
        }

        _busy = false;
    }

    private void TeleportSafe(Transform playerTransform, Transform target)
    {
        if (playerTransform == null) { Debug.LogError("[Portal] Player null."); return; }
        if (target == null) { Debug.LogError("[Portal] Spawn target null."); return; }

        var rb = playerTransform.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = target.position;
            rb.rotation = target.rotation;
            rb.Sleep();
            return;
        }

        var cc = playerTransform.GetComponent<PlayerController2>();
        if (cc != null)
        {
            cc.enabled = false;
            playerTransform.SetPositionAndRotation(target.position, target.rotation);
            cc.enabled = true;
            return;
        }
        playerTransform.SetPositionAndRotation(target.position, target.rotation);
    }

    private Transform FindSpawnInScene(string sceneName, SpawnId id)
    {
        Scene s = SceneManager.GetSceneByName(sceneName);
        if (!s.isLoaded)
        {
            Debug.LogError($"[Portal] Scene '{sceneName}' no está cargada.");
            return null;
        }

        foreach (var root in s.GetRootGameObjects())
        {
            var markers = root.GetComponentsInChildren<SpawnPointMarker>(true);
            foreach (var m in markers)
                if (m.id == id) return m.transform;
        }

        Debug.LogError($"[Portal] No encontré SpawnPointMarker '{id}' dentro de '{sceneName}'.");
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

        Debug.LogError($"[Portal] No encontré SpawnPointMarker '{id}' en escenas activas.");
        return null;
    }

    private void MoveInteriorRoots(string sceneName, Vector3 offset)
    {
        var s = SceneManager.GetSceneByName(sceneName);
        if (!s.isLoaded) return;

        foreach (var root in s.GetRootGameObjects())
        {
            if (root.name.Contains("InteriorRoot"))
            {
                root.transform.position += offset;
                return;
            }
        }

        foreach (var root in s.GetRootGameObjects())
            root.transform.position += offset;
    }
}