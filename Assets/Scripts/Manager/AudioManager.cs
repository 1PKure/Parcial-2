using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer & Volumen")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string masterParam = "MasterVolume";
    [SerializeField] private string sfxParam = "SFXVolume";
    [SerializeField] private string musicParam = "MusicVolume";


    [Header("Audio Sources & Clips")]
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

    public void PlayUIHover() => PlaySound(uiHoverClip);
    public void PlayJump() => PlaySound(jumpClip);

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    public void SetVolume(string parameter, float sliderValue)
    {
        float volume = Mathf.Log10(Mathf.Clamp(sliderValue, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat(parameter, volume);
    }

    public float GetVolume(string parameter)
    {
        if (audioMixer.GetFloat(parameter, out float volumeDb))
            return Mathf.Pow(10f, volumeDb / 20f);
        return 1f;
    }

    public void InitSlider(Slider slider, string parameter)
    {
        slider.value = GetVolume(parameter);
        slider.onValueChanged.AddListener(value => SetVolume(parameter, value));
    }

    public void AddHoverSound(Button button)
    {
        EventTrigger trigger = button.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = button.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((eventData) => { PlayUIHover(); });
        trigger.triggers.Add(entry);
    }
}
