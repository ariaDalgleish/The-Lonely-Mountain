using UnityEngine;


public enum SoundType
{
    // not finished 
    Footstep,
    Run,
    Wind,
    StartFire,
    Fire,
    Menu,
    Equipment,
    Music,
    Magic,
    Human
}

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] soundList;
    private static SoundManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(SoundType sound, float volume = 1)
    {
        // PlayOneShot gets current settings of the AudioSource
        // Plays one shot audio clip and doesn't fill the audio clip space 
        instance.audioSource.PlayOneShot(instance.soundList[(int)sound], volume);
    }
}
