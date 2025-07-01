using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject creditsPanel;

    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private AudioMixer audioMixer;

    private bool isPaused = false;
    private PlayerController2 playerController;

    private void Start()
    {

        playerController = FindObjectOfType<PlayerController2>();
        float masterVol;
        audioMixer.GetFloat("MasterVolume", out masterVol);
        masterSlider.value = Mathf.Pow(10, masterVol / 20f);

        float sfxVol;
        audioMixer.GetFloat("SFXVolume", out sfxVol);
        sfxSlider.value = Mathf.Pow(10, sfxVol / 20f);
    }

    private void Update()
    {
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
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        if (playerController != null) playerController.enabled = false;
    }

    public void CloseAllPanels()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        creditsPanel.SetActive(false);
        Time.timeScale = 1f;
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

    public void SetMasterVolume()
    {
        float volume = masterSlider.value;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetSFXVolume()
    {
        float volume = sfxSlider.value;
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}
