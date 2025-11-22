using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("References")]
    public soundlibrary soundLibrary; // Library lama kamu
    public AudioSource sfxSource;     // Drag Audio Source di sini
    public AudioMixer mixer;          // Drag Audio Mixer di sini

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("SFXVolume", 0f);
        if(mixer != null) mixer.SetFloat("SFXVolume", savedVolume);
    }

    public void SetVolume(float volume)
    {
        if(mixer != null) mixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }

    // --- FUNGSI LAMA (Play pakai String) ---
    public void Play(string soundName)
    {
        if (soundLibrary != null)
        {
            AudioClip clip = soundLibrary.GetClipFromName(soundName);
            if (clip != null && sfxSource != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }

    // --- FUNGSI BARU (INI YANG HILANG & BIKIN ERROR) ---
    // Fungsi ini dibutuhkan oleh GlobalButtonSound
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}