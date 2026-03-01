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
    [SerializeField] private CanvasGroup loadingGroup;       
    [SerializeField] private Slider fakeLoadingBar;
    [SerializeField] private TMP_Text loadingText;

    [Header("Fake Loading")]
    [SerializeField] private float minFakeDuration = 1.50f;
    [SerializeField] private float extraFakePadding = 0.25f;

    private bool isLoading;
    public bool IsLoading => isLoading;

    private void Awake()
    {
        if (transform.parent != null)
            transform.SetParent(null);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        AutoWireUI();
        HideLoadingInstant();
    }

    private void AutoWireUI()
    {
        if (loadingRoot == null)
        {
            var canvas = GetComponentInChildren<Canvas>(true);
            if (canvas != null) loadingRoot = canvas.gameObject;
        }

        if (loadingGroup == null && loadingRoot != null)
            loadingGroup = loadingRoot.GetComponent<CanvasGroup>();

        if (fakeLoadingBar == null)
            fakeLoadingBar = GetComponentInChildren<Slider>(true);

        if (loadingText == null)
            loadingText = GetComponentInChildren<TMP_Text>(true);

        if (loadingRoot != null && loadingGroup == null)
            loadingGroup = loadingRoot.AddComponent<CanvasGroup>();


        if (loadingRoot != null && !loadingRoot.activeSelf)
            loadingRoot.SetActive(true);
    }

    private void HideLoadingInstant()
    {
        if (loadingGroup != null)
        {
            loadingGroup.alpha = 0f;
            loadingGroup.blocksRaycasts = false;
            loadingGroup.interactable = false;
        }

        if (fakeLoadingBar != null) fakeLoadingBar.value = 0f;
        if (loadingText != null) loadingText.text = "";
    }

    public void LoadSceneSingle(string sceneName)
    {
        if (isLoading) return;
        StartCoroutine(CoLoadScene(sceneName, LoadSceneMode.Single));
        EnsureEventSystem();
        Time.timeScale = 1f;
    }

    public void LoadSceneWithFakeLoading(string sceneName) => LoadSceneSingle(sceneName);

    public void LoadSceneAdditive(string sceneName)
    {
        if (isLoading) return;
        if (SceneManager.GetSceneByName(sceneName).isLoaded) return;
        StartCoroutine(CoLoadScene(sceneName, LoadSceneMode.Additive));
    }

    public void UnloadScene(string sceneName)
    {
        if (isLoading) return;
        StartCoroutine(CoUnloadScene(sceneName));
    }

    private IEnumerator CoLoadScene(string sceneName, LoadSceneMode mode)
    {
        if (!Application.CanStreamedLevelBeLoaded(sceneName))
        {
            Debug.LogError($"[SceneLoader] Scene '{sceneName}' no está en Build Settings o el nombre es incorrecto.");
            yield break;
        }

        isLoading = true;
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

        float current = fakeLoadingBar != null ? fakeLoadingBar.value : 0f;
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
        while (!op.isDone) yield return null;

        SetProgress(1f, "Done!");
        yield return new WaitForSecondsRealtime(0.15f);
        SetLoading(false);
        isLoading = false;
    }

    private IEnumerator CoUnloadScene(string sceneName)
    {
        if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        {
            Debug.LogWarning($"[SceneLoader] Unload pedido pero '{sceneName}' no está cargada.");
            yield break;
        }

        isLoading = true;
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
        isLoading = false;
    }

    private void SetLoading(bool active)
    {
        AutoWireUI();

        if (loadingGroup == null) return;

        loadingGroup.alpha = active ? 1f : 0f;
        loadingGroup.blocksRaycasts = active;
        loadingGroup.interactable = active;
    }

    private void SetProgress(float value, string text = null)
    {
        if (fakeLoadingBar != null) fakeLoadingBar.value = value;
        if (loadingText != null && !string.IsNullOrEmpty(text)) loadingText.text = text;
    }

    private void EnsureEventSystem()
    {
        if (EventSystem.current != null) return;
        new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
    }
}