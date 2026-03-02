using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [Header("Audio UI")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;

    [Header("Botones con sonido")]
    [SerializeField] private Button[] buttonsWithSound;
    private bool isPaused = false;
    private PlayerController2 playerController;
    private VictoryPanelUI victoryPanel;

    private void Start()
    {
        victoryPanel = FindObjectOfType<VictoryPanelUI>(true);
        playerController = FindObjectOfType<PlayerController2>();
        AudioManager.Instance.InitSlider(masterSlider, "MasterVolume");
        AudioManager.Instance.InitSlider(sfxSlider, "SFXVolume");
        AudioManager.Instance.InitSlider(musicSlider, "MusicVolume");

        foreach (Button b in buttonsWithSound)
            AudioManager.Instance.AddHoverSound(b);
    }

    private void Update()
    {
        if (victoryPanel != null && victoryPanel.IsShowing)
            return;
        else
        {
            victoryPanel = FindObjectOfType<VictoryPanelUI>(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isPaused)
                OpenPauseMenu();
            else
                CloseAllPanels();
        }
    }

    public void OpenPauseMenu()
    {
        AudioManager.Instance?.SetMusicPaused(true);
        isPaused = true;
        pausePanel.SetActive(true);

        if (UIManager.Instance != null)
            UIManager.Instance.EnterUIMode();
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerController != null) playerController.enabled = false;
    }

    public void CloseAllPanels()
    {
        AudioManager.Instance?.SetMusicPaused(false);   

        isPaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        Time.timeScale = 1f;

        if (UIManager.Instance != null)
            UIManager.Instance.ExitUIMode();

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        if (playerController != null) playerController.enabled = true;
    }

    public void OpenSettings()
    {
        pausePanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void OpenCredits()
    {
        pausePanel.SetActive(false);
        creditsPanel.SetActive(true);
    }

    public void BackToPause()
    {
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        pausePanel.SetActive(true);
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
