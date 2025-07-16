using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip uiHoverClip;
    [SerializeField] private AudioClip jumpClip;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayUIHover()
    {
        if (uiHoverClip != null)
            sfxSource.PlayOneShot(uiHoverClip);
    }

    public void PlayJump()
    {
        if (jumpClip != null)
            sfxSource.PlayOneShot(jumpClip);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }
}
