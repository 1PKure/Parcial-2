using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public string sceneToLoad = "";
    private int collectedStones = 0;
    public int totalStones = 5;
    public GameObject specialStone;


    private void Awake()
    {
        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void LoadScene(string targetScene)
    {
        SceneLoader.Instance.LoadSceneSingle(targetScene);
    }


    public void AddMagicStone()
    {
        collectedStones++;
        UIManager.Instance.ShowMessage($"Piedras recolectadas: {collectedStones}/{totalStones}");
        UIManager.Instance.UpdateStoneUI(collectedStones);

        if (collectedStones >= totalStones)
        {
            UnlockRock();
        }
    }

    private void UnlockRock()
    {
        UIManager.Instance.ShowMessage("¡Piedras completas! Roca desbloqueada.");

        UIManager.Instance.HideStoneUI();

        if (specialStone != null)
            specialStone.SetActive(false);
    }
    public bool HasAllStones()
    {
        return collectedStones >= totalStones;
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
