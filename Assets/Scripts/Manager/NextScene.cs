using UnityEngine;

public class NextScene : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Gameplay";
    [SerializeField] private GameObject instructionText;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private bool _playerInside = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = true;

        if (instructionText != null)
            instructionText.SetActive(GameManager.Instance.HasAllStones());
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerInside = false;

        if (instructionText != null)
            instructionText.SetActive(false);
    }

    private void Update()
    {
        if (!_playerInside) return;
        if (!GameManager.Instance.HasAllStones()) return;

        if (Input.GetKeyDown(interactKey))
        {
            SceneLoader.Instance.LoadSceneSingle(nextSceneName);
        }
    }
}
