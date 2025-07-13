using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ButtonSounds : MonoBehaviour
{
    private AudioSource buttonAudio;

    [SerializeField] private AudioClip select;

    [Range(0.0f, 1.0f)]
    [SerializeField] float selectVolume;

    [SerializeField] private AudioClip hover;

    [Range(0.0f, 1.0f)]
    [SerializeField] float hoverVolume;


    protected void Awake()
    {
        buttonAudio = this.GetComponent<AudioSource>();
    }

    public void PlayHighlightSound()
    {
        this.buttonAudio.clip = this.hover;
        this.buttonAudio.volume = hoverVolume;
        this.buttonAudio.Play();
    }

    public void PlaySelectSound()
    {
        this.buttonAudio.clip = this.select;
        this.buttonAudio.volume = selectVolume;
        this.buttonAudio.Play();
    }
}
