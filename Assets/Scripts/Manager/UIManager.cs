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

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetTotalStones(int amount)
    {
        totalStones = amount;
        UpdateStoneUI(0);
    }

    public void UpdateStoneUI(int collected)
    {
        stoneText.text = $"Piedras: {collected}/{totalStones}";
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

}

