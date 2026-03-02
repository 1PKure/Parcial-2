using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Piedras mágicas")]
    [SerializeField] private TMP_Text stoneText;
    private int totalStones;

    [Header("Vida del jugador")]
    [SerializeField] private Image healthBar;

    [SerializeField] private TMP_Text messageText;
    [SerializeField] private float messageDuration = 2f;
    private Coroutine currentMessageCoroutine;

    public static bool IsUIOpen { get; private set; }
    public void EnterUIMode()
    {
        IsUIOpen = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void ExitUIMode()
    {
        IsUIOpen = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        SetTotalStones(GameManager.Instance.totalStones);

        if (GameManager.Instance.HasAllStones())
            HideStoneUI();
    }

    public void SetTotalStones(int amount)
    {
        totalStones = amount;
        UpdateStoneUI(0);
    }

    public void UpdateStoneUI(int collected)
    {
        if (collected >= totalStones)
        {
            HideStoneUI();
            return;
        }

        stoneText.text = $"Piedras: {collected}/{totalStones}";
        if (!stoneText.gameObject.activeSelf)
            stoneText.gameObject.SetActive(true);
    }

    public void UpdateHealthUI(float current, float max)
    {
        healthBar.fillAmount = current / max;
    }

    public void ShowMessage(string text)
    {
        if (currentMessageCoroutine != null)
            StopCoroutine(currentMessageCoroutine);

        currentMessageCoroutine = StartCoroutine(ShowMessageRoutine(text));
    }

    private IEnumerator ShowMessageRoutine(string text)
    {
        messageText.text = text;
        messageText.gameObject.SetActive(true);

        yield return new WaitForSeconds(messageDuration);

        messageText.gameObject.SetActive(false);
    }

    public void HideStoneUI()
    {
        if (stoneText != null)
            stoneText.gameObject.SetActive(false);
    }

    public void ShowStoneUI()
    {
        if (stoneText != null)
            stoneText.gameObject.SetActive(true);
    }
}

