using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioSource sfxSource;

    private float masterVolume = 1f;
    private float sfxVolume = 1f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetMasterVolume(float volume)
    {
        masterVolume = volume;
        AudioListener.volume = masterVolume; // GLOBAL
    }

    public void PlayClick()
    {
        sfxSource.PlayOneShot(sfxSource.clip, sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }

    public float GetSFXVolume()
    {
        return sfxVolume;
    }

    void Start()
    {
        GetComponent<AudioSource>().Play();
    }
}