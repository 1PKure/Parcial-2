using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteriorPortalGate : MonoBehaviour
{
    [SerializeField] private InteriorLevelManager level;
    [SerializeField] private GhostController ghost;

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        if (level == null) level = FindObjectOfType<InteriorLevelManager>();
        if (ghost == null) ghost = FindObjectOfType<GhostController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (level != null && !level.AllWizardsDefeated)
        {
            Debug.Log("No podés salir: faltan magos por derrotar.");
            return;
        }

        if (ghost != null && ghost.IsPossessing)
        {
            Debug.Log("No podés salir poseído: volvé a tu cuerpo.");
            return;
        }

        Debug.Log("OK: portal habilitado, avanzar a la siguiente etapa.");
    }
}