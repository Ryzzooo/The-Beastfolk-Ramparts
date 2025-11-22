using UnityEngine;
using UnityEngine.UI;

public class ToggleButtonController : MonoBehaviour
{
    public enum ButtonType { PlayPauseGame, AudioVolume, GameSpeed }

    [Header("Settings")]
    public ButtonType buttonType;
    
    [Tooltip("Nama parameter di Animator (misal: isAudioOn / onPlay)")]
    public string animParameter = "onPlay"; 
    public Animator buttonAnimator;

    [Header("Audio FX (Opsional)")]
    public AudioClip clickClip; 

    // Logic State
    private bool isStateOn = true;

    void Start()
    {
        if (buttonAnimator == null) buttonAnimator = GetComponent<Animator>();
        
        // 1. Cek Status Terakhir (Visual)
        RefreshVisualState();

        // 2. [FIX PENTING] Paksa update Suara/Speed saat game baru mulai!
        // Biar sinkron antara Gambar dan Suara aslinya.
        if (buttonType == ButtonType.AudioVolume)
        {
            ApplyAudioToManager(); 
        }
        else if (buttonType == ButtonType.GameSpeed)
        {
            // Sinkronkan speed juga kalau perlu
            if (isStateOn && Time.timeScale > 0) Time.timeScale = 2f;
        }
    }

    public void OnButtonClick()
    {
        // Mainkan Suara Klik (Smart Sound)
        PlaySmartSound();

        // Balik Status (Toggle)
        isStateOn = !isStateOn;

        switch (buttonType)
        {
            case ButtonType.PlayPauseGame:
                Pausemenu pauseMenu = Object.FindFirstObjectByType<Pausemenu>();
                if (pauseMenu != null) pauseMenu.TogglePlayPauseOnly();
                break;

            case ButtonType.GameSpeed:
                float targetSpeed = isStateOn ? 2f : 1f;
                Pausemenu.savedSpeed = targetSpeed;
                if (Time.timeScale > 0) Time.timeScale = targetSpeed;
                RefreshVisualState();
                break;

            case ButtonType.AudioVolume:
                // Simpan & Terapkan Audio
                SaveAudioState();
                ApplyAudioToManager();
                RefreshVisualState();
                break;
        }
    }

    // --- FUNGSI UPDATE VISUAL ---
    public void RefreshVisualState()
    {
        switch (buttonType)
        {
            case ButtonType.PlayPauseGame: 
                isStateOn = (Time.timeScale > 0); 
                break;
            case ButtonType.GameSpeed: 
                isStateOn = (Pausemenu.savedSpeed > 1.5f); 
                break;
            case ButtonType.AudioVolume: 
                // Ambil data (Default 1/Nyala)
                isStateOn = PlayerPrefs.GetInt("MusicMuted", 1) == 1; 
                break;
        }
        if (buttonAnimator != null) buttonAnimator.SetBool(animParameter, isStateOn);
    }
    
    // --- FUNGSI SIMPAN DATA ---
    private void SaveAudioState()
    {
         int status = isStateOn ? 1 : 0;
         PlayerPrefs.SetInt("MusicMuted", status);
         PlayerPrefs.SetInt("SFXMuted", status);
         PlayerPrefs.Save();
    }

    // --- FUNGSI TERAPKAN VOLUME (Dipanggil di Start & Click) ---
    private void ApplyAudioToManager()
    {
         float musicVol, sfxVol;

         if (isStateOn) // KONDISI NYALA (UNMUTE)
         {
             // Ambil volume terakhir (Default 1f = Full)
             musicVol = PlayerPrefs.GetFloat("MusicVolume", 1f); 
             sfxVol = PlayerPrefs.GetFloat("SFXVolume", 1f);

             // Penyelamat: Kalau data 0/error, paksa Full
             if (musicVol <= 0.001f) musicVol = 1f; 
             if (sfxVol <= 0.001f) sfxVol = 1f;
         }
         else // KONDISI MATI (MUTE)
         {
             musicVol = -80f;
             sfxVol = -80f;
         }

         // Kirim ke Manager
         if (MusicManager.Instance != null) MusicManager.Instance.SetVolume(musicVol);
         if (SoundManager.Instance != null) SoundManager.Instance.SetVolume(sfxVol);
    }

    // --- LOGIKA SUARA KLIK ---
    private void PlaySmartSound()
    {
        AudioClip clipToPlay = clickClip;
        if (clipToPlay == null)
        {
            var globalSound = Object.FindFirstObjectByType<GlobalUISound>();
            if (globalSound != null) clipToPlay = globalSound.standardClickSound;
        }

        if (SoundManager.Instance != null && clipToPlay != null)
        {
            SoundManager.Instance.PlaySFX(clipToPlay);
        }
    }
}