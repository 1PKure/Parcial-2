using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InteriorPortalGate : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private InteriorLevelManager level;
    [SerializeField] private VictoryPanelUI victoryPanel;

    [Header("Possesing")]
    [SerializeField] private bool blockIfPossessing = true;
    [SerializeField] private GhostController ghost;

    private Collider col;
    private float lastBlockedMsgTime;
    private UIManager uiManager;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;

        if (level == null) level = FindObjectOfType<InteriorLevelManager>(true);
        if (victoryPanel == null) victoryPanel = FindObjectOfType<VictoryPanelUI>(true);
        if (uiManager == null) uiManager = FindObjectOfType<UIManager>(true);
        if (ghost == null) ghost = FindObjectOfType<GhostController>(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponentInParent<PlayerController2>();
        if (player == null) return;

        if (level != null && !level.AllWizardsDefeated)
        {
            TryBlockedMessage("Portal: bloqueado");
            return;
        }

        if (blockIfPossessing && ghost != null && ghost.IsPossessing)
        {
            TryBlockedMessage("Volvé a tu cuerpo para usar el portal");
            return;
        }

        if (victoryPanel != null) victoryPanel.Show();
        else Debug.LogError("[Portal] No hay VictoryPanelUI en escena.");
    }

    private void TryBlockedMessage(string msg)
    {
        if (Time.time - lastBlockedMsgTime < 1f) return;
        lastBlockedMsgTime = Time.time;

        if (uiManager != null) uiManager.ShowMessage(msg);
        else Debug.Log(msg);
    }
}