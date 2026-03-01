using UnityEngine;

public class InteriorLevelManager : MonoBehaviour
{
    public int RemainingWizards { get; private set; }
    public bool AllWizardsDefeated => RemainingWizards <= 0;

    private WizardHealth[] wizards;

    private void Start()
    {
        wizards = FindObjectsOfType<WizardHealth>(true);

        RemainingWizards = 0;
        foreach (var w in wizards)
        {
            if (w == null) continue;
            if (w.IsDead) continue;

            RemainingWizards++;
            w.OnDied += HandleWizardDied;
        }

        Debug.Log($"[InteriorLevelManager] Wizards vivos: {RemainingWizards}");
    }

    private void HandleWizardDied(WizardHealth w)
    {
        RemainingWizards = Mathf.Max(0, RemainingWizards - 1);
        Debug.Log($"[InteriorLevelManager] Wizard muerto. Restantes: {RemainingWizards}");
    }
}