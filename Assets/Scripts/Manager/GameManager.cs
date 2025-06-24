using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string sceneToLoad = "";
    private int collectedStones = 0;
    public int totalStones = 5;
    public GameObject specialRock;

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

    public void LoadScene(string targetScene)
    {
        SceneLoader.Instance.LoadSceneWithFakeLoading(targetScene);
    }
    public void OnStartGame()
    {
        GameManager.Instance.LoadScene("Gameplay");
    }

    public void AddMagicStone()
    {
        collectedStones++;
        Debug.Log($"Piedras recolectadas: {collectedStones}/{totalStones}");

        if (collectedStones >= totalStones)
        {
            UnlockRock();
        }
    }

    private void UnlockRock()
    {
        Debug.Log("¡Piedras completas! Roca desbloqueada.");
        if (specialRock != null)
            specialRock.SetActive(false); // o animación, partículas, etc.
    }

    public void ResetStones()
    {
        collectedStones = 0;
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
