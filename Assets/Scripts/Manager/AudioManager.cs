using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Play3DSound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        GameObject tempGO = new GameObject("TempAudio");
        tempGO.transform.position = position;

        AudioSource aSource = tempGO.AddComponent<AudioSource>();
        aSource.clip = clip;
        aSource.spatialBlend = 1f; // 3D Sound
        aSource.volume = volume;
        aSource.rolloffMode = AudioRolloffMode.Linear;
        aSource.minDistance = 1f;
        aSource.maxDistance = 50f;
        aSource.Play();

        Destroy(tempGO, clip.length);
    }
}

