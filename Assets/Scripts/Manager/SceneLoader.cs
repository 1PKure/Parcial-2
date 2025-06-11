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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(GameManager.Instance.sceneToLoad))
        {
            StartCoroutine(LoadSceneAsync(GameManager.Instance.sceneToLoad));
        }
        else
        {
            Debug.LogWarning("No hay escena objetivo para cargar.");
        }
    }
    public void StartFakeLoad(string targetScene)
    {
        StartCoroutine(LoadSceneAsync(targetScene));
    }
    public IEnumerator LoadSceneAsync(string targetScene)
    {
        loadingScreen.SetActive(true);
        fakeLoadingBar.value = 0;

        float elapsed = 0;
        while (elapsed < fakeDuration)
        {
            elapsed += Time.deltaTime;
            fakeLoadingBar.value = Mathf.Clamp01(elapsed / fakeDuration);
            yield return null;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        asyncLoad.allowSceneActivation = false;

        while (asyncLoad.progress < 0.9f)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        asyncLoad.allowSceneActivation = true;
    }
}
