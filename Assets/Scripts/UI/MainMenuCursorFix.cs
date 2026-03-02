using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCursorFix : MonoBehaviour
{
    private void OnEnable()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
