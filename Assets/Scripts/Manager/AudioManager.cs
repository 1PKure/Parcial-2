using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer & Volumen")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioClip defaultGameplayMusic;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioClip ambienceClip;
    [SerializeField] private float musicCrossfadeSeconds = 0.75f;
    [SerializeField] private string musicMixerParam = "MusicVolume";
    [SerializeField] private float defaultMusicSliderValue = 1f;

    [Header("Audio Sources & Clips")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip uiHoverClip;
    [SerializeField] private AudioClip jumpClip;

    private Coroutine musicCoroutine;


    private void Awake()
    {
        if (transform.parent != null)
            transform.SetParent(null);

        DontDestroyOnLoad(gameObject);

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (musicSource == null)
            musicSource = GetComponentInChildren<AudioSource>(true);

        if (musicSource == null)
            musicSource = gameObject.AddComponent<AudioSource>();

        musicSource.playOnAwake = false;
        musicSource.loop = true;
        musicSource.spatialBlend = 0f;
        ambienceSource.spatialBlend = 1f;

        if (ambienceSource != null && ambienceClip != null)
        {
            ambienceSource.clip = ambienceClip;
            ambienceSource.loop = true;
            ambienceSource.playOnAwake = false;
            ambienceSource.spatialBlend = 1f;
            ambienceSource.Play();
        }
    }
    private void Start()
    {
        if (audioMixer != null)
        {
            float v = PlayerPrefs.HasKey(musicMixerParam) ? PlayerPrefs.GetFloat(musicMixerParam) : defaultMusicSliderValue;
            SetVolume(musicMixerParam, v);
        }

        if (defaultGameplayMusic != null)
            PlayMusic(defaultGameplayMusic, loop: true, volume: 1f);

    }
    public void PlayUIHover() => PlaySound(uiHoverClip);
    public void PlayJump() => PlaySound(jumpClip);
    public void SetMusicPaused(bool paused)
    {
        if (musicSource != null)
        {
            if (paused) musicSource.Pause();
            else musicSource.UnPause();
        }

        if (ambienceSource != null)
        {
            if (paused) ambienceSource.Pause();
            else ambienceSource.UnPause();
        }
    }
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

    public void PlayMusic(AudioClip clip, bool loop = true, float volume = 1f)
    {
        if (musicSource == null || clip == null) return;

        if (musicSource.clip == clip && musicSource.isPlaying) return;

        if (musicCoroutine != null)
            StopCoroutine(musicCoroutine);

        musicCoroutine = StartCoroutine(CoCrossfadeMusic(clip, loop, volume));
    }

    private IEnumerator CoCrossfadeMusic(AudioClip nextClip, bool loop, float targetVolume)
    {
        float startVol = musicSource.volume;

        for (float t = 0f; t < musicCrossfadeSeconds; t += Time.unscaledDeltaTime)
        {
            float k = t / musicCrossfadeSeconds;
            musicSource.volume = Mathf.Lerp(startVol, 0f, k);
            yield return null;
        }

        musicSource.clip = nextClip;
        musicSource.loop = loop;
        musicSource.spatialBlend = 0f;
        musicSource.Play();

        for (float t = 0f; t < musicCrossfadeSeconds; t += Time.unscaledDeltaTime)
        {
            float k = t / musicCrossfadeSeconds;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, k);
            yield return null;
        }

        musicSource.volume = targetVolume;
        musicCoroutine = null;
    }
}
