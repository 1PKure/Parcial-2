using System;
using System.Collections.Generic;
using UnityEngine;

public class InteriorLevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InteriorHUDUI hudUI;


    [Header("Wizards")]
    [SerializeField] private bool autoFindWizardsOnStart = true;

    public int RemainingWizards { get; private set; }
    public bool AllWizardsDefeated => RemainingWizards <= 0;

    public event Action OnAllWizardsDefeated;

    private readonly HashSet<WizardHealth> tracked = new();
    private bool firedAllDead;
    private bool lastPortalBlockedState = true;
    private UIManager uiManager;

    private void Awake()
    {
        if (hudUI == null) hudUI = FindObjectOfType<InteriorHUDUI>(true);
    }

    private void Start()
    {
        if (autoFindWizardsOnStart)
            RegisterAllWizardsInScene();

        PushHUD();
        PushPortalStateMessage(force: true);
    }

    public void RegisterAllWizardsInScene()
    {
        RemainingWizards = 0;
        tracked.Clear();

        var wizards = FindObjectsOfType<WizardHealth>(true);
        foreach (var w in wizards)
            RegisterWizard(w);

        Debug.Log($"[InteriorLevelManager] Wizards vivos: {RemainingWizards}");
    }

    public void RegisterWizard(WizardHealth w)
    {
        if (w == null) return;
        if (tracked.Contains(w)) return;

        tracked.Add(w);

        if (!w.IsDead) RemainingWizards++;
        w.OnDied += HandleWizardDied;

        PushHUD();
        PushPortalStateMessage();
    }

    private void HandleWizardDied(WizardHealth w)
    {
        RemainingWizards = Mathf.Max(0, RemainingWizards - 1);

        PushHUD();
        PushPortalStateMessage();

        if (!firedAllDead && AllWizardsDefeated)
        {
            firedAllDead = true;

            OnAllWizardsDefeated?.Invoke();
        }
    }

    private void PushHUD()
    {
        if (hudUI != null)
        {
            hudUI.SetEnemiesRemaining(RemainingWizards);
            hudUI.SetPortalBlocked(!AllWizardsDefeated);
        }
    }

    private void PushPortalStateMessage(bool force = false)
    {
        bool blocked = !AllWizardsDefeated;
        if (!force && blocked == lastPortalBlockedState) return;

        lastPortalBlockedState = blocked;

        if (uiManager != null)
            uiManager.ShowMessage(blocked ? "Portal: bloqueado" : "Portal: desbloqueado");
    }

    private void OnDestroy()
    {
        foreach (var w in tracked)
            if (w != null) w.OnDied -= HandleWizardDied;
        tracked.Clear();
    }
}