using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextSceneClickTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Gameplay";
    [SerializeField] private GameObject instructionText;
    private void OnMouseEnter()
    {
        if (!GameManager.Instance.HasAllStones()) return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (instructionText != null)
            instructionText.SetActive(true);
    }

    private void OnMouseExit()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (instructionText != null)
            instructionText.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!GameManager.Instance.HasAllStones()) return;

        SceneLoader.Instance.LoadSceneWithFakeLoading(nextSceneName);
    }
}

