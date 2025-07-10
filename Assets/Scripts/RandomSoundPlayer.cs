using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to play a random sound from the list.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class RandomSoundPlayer : MonoBehaviour
{
    public List<AudioClip> Clips;
    private AudioSource audioSource;

    private void Awake()
    {
        this.audioSource = this.GetComponent<AudioSource>();
    }

    public void PlayRandomSound()
    {
        audioSource.clip = Clips[Random.Range(0, Clips.Count)];
        audioSource.Play();
    }
}
