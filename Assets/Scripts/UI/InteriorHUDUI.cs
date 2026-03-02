using UnityEngine;
using TMPro;

public class InteriorHUDUI : MonoBehaviour
{
    [SerializeField] private TMP_Text enemiesText;
    [SerializeField] private TMP_Text portalText;

    public void SetEnemiesRemaining(int remaining)
    {
        if (enemiesText != null)
            enemiesText.text = $"Enemigos: {remaining}";
    }

    public void SetPortalBlocked(bool blocked)
    {
        if (portalText != null)
            portalText.text = blocked ? "Portal: bloqueado" : "Portal: desbloqueado";
    }
}