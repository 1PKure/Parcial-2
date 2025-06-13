using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider fakeLoadingBar;
    [SerializeField] private float fakeDuration = 3f;
    [SerializeField] private Canvas Canvas;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadSceneWithFakeLoading(string sceneName)
    {
        if (loadingScreen == null || fakeLoadingBar == null)
        {
            Debug.LogError("SceneLoader: loadingScreen o fakeLoadingBar no están asignados.");
            return;
        }
        StartCoroutine(LoadSceneWithFakeBar(sceneName));
    }

    private IEnumerator LoadSceneWithFakeBar(string sceneName)
    {
        loadingScreen.SetActive(true);
        fakeLoadingBar.gameObject.SetActive(true);
        Canvas.enabled = false;
        fakeLoadingBar.value = 0;

        AsyncOperation realLoad = SceneManager.LoadSceneAsync(sceneName);
        realLoad.allowSceneActivation = false;

        float elapsed = 0f;

        while (elapsed < fakeDuration)
        {
            elapsed += Time.deltaTime;
            float fakeProgress = Mathf.Clamp01(elapsed / fakeDuration);
            fakeLoadingBar.value = fakeProgress;

            if (realLoad.progress >= 0.9f && fakeProgress >= 0.99f)
                break;

            yield return null;
        }
        yield return new WaitForSeconds(0.3f);

        fakeLoadingBar.value = 1f;

        realLoad.allowSceneActivation = true;
        while (!realLoad.isDone)
            yield return null;

        if (loadingScreen != null)
            loadingScreen.SetActive(false);

        if (fakeLoadingBar != null)
            fakeLoadingBar.gameObject.SetActive(false);
    }
}