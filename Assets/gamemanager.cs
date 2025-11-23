using UnityEngine;
using UnityEngine.SceneManagement; // Penting untuk ganti/reload scene
using UnityEngine.UI; // Untuk memanipulasi UI (opsional jika pakai Button via Inspector)

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoryPanel;

    [Header("Next Level Name")]
    [Tooltip("Tulis nama scene selanjutnya di sini (kosongkan jika ingin otomatis ke index berikutnya)")]
    [SerializeField] private string nextSceneName;

    private bool isGameEnded = false;

    void Awake()
    {
        // Pola Singleton sederhana
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Pastikan waktu berjalan normal saat game mulai (karena kita stop waktu saat kalah/menang)
        Time.timeScale = 1f;
        isGameEnded = false;

        // Sembunyikan panel di awal
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (victoryPanel != null) victoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        // Subscribe ke event dari script PlayerHealth dan Spawner kamu
        PlayerHealth.OnPlayerDefeated += HandleGameOver;
        Spawner.OnLevelCompleted += HandleVictory;
    }

    private void OnDisable()
    {
        // Unsubscribe untuk mencegah memory leak
        PlayerHealth.OnPlayerDefeated -= HandleGameOver;
        Spawner.OnLevelCompleted -= HandleVictory;
    }

    // --- LOGIKA MENANG & KALAH ---

    private void HandleGameOver()
    {
        if (isGameEnded) return; // Mencegah terpanggil double
        isGameEnded = true;

        Debug.Log("Game Over! Memunculkan panel kalah.");
        
        // Matikan waktu agar musuh berhenti bergerak
        Time.timeScale = 0f;

        // Munculkan UI
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    private void HandleVictory()
    {
        if (isGameEnded) return;
        isGameEnded = true;

        Debug.Log("Victory! Memunculkan panel menang.");
        
        // Matikan waktu
        Time.timeScale = 0f;

        // Munculkan UI
        if (victoryPanel != null) victoryPanel.SetActive(true);
    }

    // --- FUNGSI UNTUK BUTTON UI ---

    // Pasang fungsi ini di Button Restart
    public void RestartLevel()
    {
        // Mengambil nama scene yang sedang aktif sekarang
        Scene currentScene = SceneManager.GetActiveScene();
        
        // Reload ulang scene-nya. 
        // Ini otomatis mereset Coin, Turret, Wave, dan Health ke awal.
        SceneManager.LoadScene(currentScene.name);
    }

    // Pasang fungsi ini di Button Next Stage
    public void GoToNextLevel()
    {
        // Jika nama scene spesifik diisi di inspector
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            // Jika tidak, load index scene berikutnya di Build Settings
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            
            // Cek apakah ada scene selanjutnya
            if (currentSceneIndex + 1 < SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene(currentSceneIndex + 1);
            }
            else
            {
                Debug.Log("Ini adalah level terakhir! Kembali ke Main Menu (Contoh).");
                // SceneManager.LoadScene("MainMenu"); // Opsional
            }
        }
    }

    // Pasang ini di Button Quit/Menu (Opsional)

}