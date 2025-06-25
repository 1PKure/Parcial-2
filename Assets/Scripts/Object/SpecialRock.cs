using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpecialRock : MonoBehaviour
{
    [SerializeField] private string nextSceneName;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManager.Instance.HasAllStones())
            {
                SceneLoader.Instance.LoadSceneWithFakeLoading(nextSceneName);
            }
            else
            {
                UIManager.Instance.ShowMessage("Aún te faltan piedras mágicas.");
            }
        }
    }
}
