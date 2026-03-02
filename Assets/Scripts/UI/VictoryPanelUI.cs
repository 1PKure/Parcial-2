using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryPanelUI : MonoBehaviour
{
    [SerializeField] private GameObject root;
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private PlayerController2 playerController;

    public bool IsShowing => root != null && root.activeSelf;

    private void Awake()
    {
        if (root == null) root = gameObject;
        root.SetActive(false);

        playerController = FindObjectOfType<PlayerController2>(true);
    }

    public void Show()
    {
        root.SetActive(true);

        Time.timeScale = 0f;

        if (UIManager.Instance != null)
            UIManager.Instance.EnterUIMode();

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController2>(true);

        if (playerController != null)
            playerController.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;

        if (UIManager.Instance != null)
            UIManager.Instance.ExitUIMode();

        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}