using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UISoundPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    private AudioSource source;

    private void Awake()
    {
        source = FindObjectOfType<AudioManager>()?.GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {
        if (clickSound != null && source != null)
            source.PlayOneShot(clickSound);
    }
}

