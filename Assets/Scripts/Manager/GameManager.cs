using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public string sceneToLoad = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void LoadScene(string targetScene)
    {
        sceneToLoad = targetScene;
        StartCoroutine(SceneLoader.Instance.LoadSceneAsync(targetScene));
    }
    private IEnumerator LoadWithLoadingScene(string targetScene)
    {
        AsyncOperation loadingScene = SceneManager.LoadSceneAsync("LoadingScene");
        yield return new WaitUntil(() => loadingScene.isDone);

        SceneLoader.Instance.StartFakeLoad(targetScene);
    }

    public void OnStartGame()
    {
        GameManager.Instance.LoadScene("Gameplay");
    }

    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
