using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pausemenu : MonoBehaviour
{
    public static float savedSpeed = 1f;

    [Header("UI References")]
    public GameObject pausePanel; 

    [Header("Audio Settings")]
    public Slider musicSlider;
    public Slider sfxSlider;

    // --- STATE VARIABLES ---
    private bool isPaused = false; 
    private bool wasPausedBeforeMenu = false; 

    // 1. Gunakan Awake untuk Reset Speed SEBELUM game jalan
    void Awake()
    {
        savedSpeed = 1f; // Pastikan speed balik ke 1x (Normal) setiap ganti scene
        Time.timeScale = savedSpeed; 
        isPaused = false; 
    }

    void Start()
    {
        if(pausePanel != null) pausePanel.SetActive(false);

        // Setup Volume
        if (musicSlider != null) musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        if (sfxSlider != null) sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        
        if (musicSlider != null && MusicManager.Instance != null) 
            musicSlider.onValueChanged.AddListener(MusicManager.Instance.SetVolume);
        if (sfxSlider != null && SoundManager.Instance != null) 
            sfxSlider.onValueChanged.AddListener(SoundManager.Instance.SetVolume);

        SyncExternalButton();
    }

    // --- 2. TOMBOL PLAY/PAUSE (HUD - Segitiga) ---
    public void TogglePlayPauseOnly()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; 
        }
        else
        {
            Time.timeScale = savedSpeed; 
            if(pausePanel != null) pausePanel.SetActive(false);
        }
        SyncExternalButton();
    }

    // --- 3. TOMBOL SETTING/MENU (HUD - Gerigi) ---
    public void OpenMenu()
    {
        wasPausedBeforeMenu = isPaused; // Simpan memori: "Tadi lagi pause atau main?"
        
        isPaused = true;
        Time.timeScale = 0f;
        
        if(pausePanel != null) pausePanel.SetActive(true);
        SyncExternalButton();
    }

    // --- 4. TOMBOL RESUME (DI DALAM MENU) ---
    public void ResumeFromMenu()
    {
        if(pausePanel != null) pausePanel.SetActive(false);

        // Logika Memori: Balik ke status asal sebelum buka menu
        if (wasPausedBeforeMenu == true)
        {
            // Dulu emang pause, ya udah tetep diam (Pause)
        }
        else
        {
            // Dulu lagi main, gas lanjut main!
            isPaused = false;
            Time.timeScale = savedSpeed;
        }

        SyncExternalButton();
    }

    // --- UTILITY ---
    private void SyncExternalButton()
    {
        // Cari tombol Toggle di scene (Unity 2023+)
        var externalBtn = Object.FindFirstObjectByType<ToggleButtonController>();
        
        // Kalau Unity versi lama pakai: 
        // var externalBtn = Object.FindObjectOfType<ToggleButtonController>();
        
        if (externalBtn != null) externalBtn.RefreshVisualState();
    }
    
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}