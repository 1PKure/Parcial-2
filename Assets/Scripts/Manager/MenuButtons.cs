using UnityEngine;

public class MenuButtons : MonoBehaviour
{
    private SceneLoader L()
    {
        if (SceneLoader.Instance == null)
        {
            Debug.LogError("[MenuButtons] SceneLoader.Instance es NULL. Falta Bootstrapper o prefab no asignado.");
            return null;
        }
        return SceneLoader.Instance;
    }

    public void Play()
    {
        var l = L(); if (l == null) return;
        l.LoadSceneSingle("Gameplay");
    }

    public void Credits()
    {
        var l = L(); if (l == null) return;
        l.LoadSceneSingle("Credits");
    }

    public void BackToMenu()
    {
        var l = L(); if (l == null) return;
        l.LoadSceneSingle("MainMenu");
    }

    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
