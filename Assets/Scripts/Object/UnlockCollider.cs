using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockCollider : MonoBehaviour
{
    [SerializeField] private Collider blockingCollider;
    [SerializeField] private string missingMsg = "Aún te faltan piedras mágicas.";

    private bool messageShown;

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        bool hasAll = GameManager.Instance.HasAllStones();

        if (!hasAll)
        {
            if (!messageShown)
            {
                UIManager.Instance.ShowMessage(missingMsg);
                messageShown = true;
            }

            if (blockingCollider != null) blockingCollider.enabled = true;
        }
        else
        {
            if (blockingCollider != null) blockingCollider.enabled = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        messageShown = false;
    }
}
