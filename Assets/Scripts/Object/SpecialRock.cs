using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpecialRock : MonoBehaviour
{
    [SerializeField] private Collider blockingCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!GameManager.Instance.HasAllStones())
        {
            UIManager.Instance.ShowMessage("Aún te faltan piedras mágicas.");
        }
        else
        {
            blockingCollider.enabled = false;
        }
    }
}

