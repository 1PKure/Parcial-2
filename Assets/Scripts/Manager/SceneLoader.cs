using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameObject loadingRoot;
    [SerializeField] private Slider fakeLoadingBar;
    [SerializeField] private TMP_Text loadingText;

    [Header("Fake Loading")]
    [SerializeField] private float minFakeDuration = 1.50f; 
    [SerializeField] private float extraFakePadding = 0.25f; 

    private void Awake()
    {
        Debug.Log($"[SceneLoader] Awake en escena: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name} - gameObject: {name}");

        if (transform.parent != null)
        {
            Debug.LogWarning("[SceneLoader] WARNING: SceneLoader NO es root. DontDestroyOnLoad falla si no es root.");
            transform.SetParent(null);
        }

        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[SceneLoader] Duplicado detectado. Destruyendo este.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        SetLoading(false);
    }

    public void LoadSceneSingle(string sceneName)
    {
        StartCoroutine(CoLoadScene(sceneName, LoadSceneMode.Single));
        EnsureEventSystem();
        Time.timeScale = 1f;
    }

    public void LoadSceneWithFakeLoading(string sceneName)
    {
        LoadSceneSingle(sceneName);
    }
    public void LoadSceneAdditive(string sceneName)
    {
        StartCoroutine(CoLoadScene(sceneName, LoadSceneMode.Additive));
    }

    public void UnloadScene(string sceneName)
    {
        StartCoroutine(CoUnloadScene(sceneName));
    }

    private IEnumerator CoLoadScene(string sceneName, LoadSceneMode mode)
    {
        
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[SceneLoader] Scene '{sceneName}' no está en Build Settings o el nombre es incorrecto.");
            yield break;
        }

        SetLoading(true);
        SetProgress(0f, $"Loading {sceneName}...");

        float startTime = Time.unscaledTime;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName, mode);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            float t = Mathf.Clamp01(op.progress / 0.9f);
            
            float target = Mathf.Lerp(0f, 0.85f, t);
            SetProgress(target);
            yield return null;
        }

      
        float elapsed = Time.unscaledTime - startTime;
        float remaining = Mathf.Max(0f, minFakeDuration - elapsed);
        float fakeTime = remaining + extraFakePadding;

        float current = fakeLoadingBar.value;
        float timer = 0f;
        while (timer < fakeTime)
        {
            timer += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(timer / fakeTime);
            float target = Mathf.Lerp(current, 1f, t);
            SetProgress(target);
            yield return null;
        }

      
        op.allowSceneActivation = true;

   
        while (!op.isDone)
            yield return null;

        SetProgress(1f, "Done!");
        yield return new WaitForSecondsRealtime(0.15f);
        SetLoading(false);
    }

    private IEnumerator CoUnloadScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.LogWarning($"[SceneLoader] Unload pedido pero '{sceneName}' no está cargada.");
            yield break;
        }

        SetLoading(true);
        SetProgress(0f, $"Unloading {sceneName}...");

        AsyncOperation op = SceneManager.UnloadSceneAsync(sceneName);

        while (!op.isDone)
        {
            SetProgress(Mathf.Clamp01(op.progress));
            yield return null;
        }

        SetProgress(1f, "Done!");
        yield return new WaitForSecondsRealtime(0.1f);
        SetLoading(false);
    }

    private void SetLoading(bool active)
    {
        if (loadingRoot != null)
            loadingRoot.SetActive(active);
    }

    private void SetProgress(float value, string text = null)
    {
        if (fakeLoadingBar != null)
            fakeLoadingBar.value = value;

        if (loadingText != null && !string.IsNullOrEmpty(text))
            loadingText.text = text;
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }


}
