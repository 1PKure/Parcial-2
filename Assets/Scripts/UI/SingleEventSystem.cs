using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SingleEventSystem : MonoBehaviour
{
    private void Awake()
    {
        Scene interiorScene = gameObject.scene;

        var systems = FindObjectsOfType<EventSystem>(true);
        if (systems.Length <= 1) return;

        foreach (var es in systems)
        {
            bool isInterior = es.gameObject.scene == interiorScene;
            es.gameObject.SetActive(isInterior);
        }
    }
}
