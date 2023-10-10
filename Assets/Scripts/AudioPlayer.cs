using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] AudioClip drawSound;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();  
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    public void PlayDrawSound()
    {
        audioSource.PlayOneShot(drawSound);
    }
}
