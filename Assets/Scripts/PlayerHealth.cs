using UnityEngine;
using UnityEngine.UI; // PENTING: Tambahkan ini untuk menggunakan komponen Image
using System;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    // Health awal dan maksimum Player
    [SerializeField] private float initialPlayerHealth = 20f;
    [SerializeField] private float maxPlayerHealth = 20f;
    
    // Damage yang diberikan ke Player jika musuh lolos.
    [SerializeField] private float damageOnEnemyPass = 1f;

    // Tambahkan referensi ke Image Health Bar Player di UI
    [Header("UI Reference")]
    [SerializeField] private Image playerHealthBarImage; 

    [Header("Damage Effects")] // <--- PASTIKAN INI ADA
    [SerializeField] private RedFlashEffect redFlashEffect; 
    [SerializeField] private float shakeDuration = 0.2f;   // <--- PASTIKAN INI ADA
    [SerializeField] private float shakeMagnitude = 0.1f;  // <--- PASTIKAN INI ADA
    // Tambahkan referensi Text/TextMeshPro
    [SerializeField] private TextMeshProUGUI healthText;

    public float CurrentHealth { get; private set; }
    
    // Event opsional untuk UI
    public static Action<float> OnHealthChanged;
    public static Action OnPlayerDefeated;

    void Awake()
    {
        CurrentHealth = initialPlayerHealth;
        // Pastikan bar HP terisi penuh saat game dimulai
        if (playerHealthBarImage != null)
        {
            playerHealthBarImage.fillAmount = 1f;
        }
        UpdateHealthText();
    }

    // File: PlayerHealth.cs

private void UpdateHealthText()
{
    if (healthText != null)
    {

        int current = Mathf.CeilToInt(CurrentHealth);
        int max = Mathf.CeilToInt(maxPlayerHealth);
        // Format teks agar terlihat seperti "15 / 20"
        healthText.text = $"{CurrentHealth} / {maxPlayerHealth}";
        
        // Opsi sederhana: Tampilkan hanya current health
        // healthText.text = CurrentHealth.ToString();
    }
}

    private void OnEnable()
    {
        // Berlangganan (Subscribe) ke event ketika musuh mencapai titik akhir
        Enemy.OnEndReached += EnemyPassed; 
    }

    private void OnDisable()
    {
        // Berhenti berlangganan (Unsubscribe) ketika objek ini dinonaktifkan
        Enemy.OnEndReached -= EnemyPassed;
    }

    // Dipanggil ketika musuh lolos
    private void EnemyPassed(Enemy passedEnemy)
    {
        TakeDamage(damageOnEnemyPass); 
        Debug.Log($"Musuh lolos! Player Health berkurang. Sisa Health: {CurrentHealth}");
    }

    public void TakeDamage(float amount)
    {
        if (CurrentHealth <= 0) return;

        CurrentHealth -= amount;
        
        if (CurrentHealth < 0)
        {
            CurrentHealth = 0;
        }

        // 1. Efek Layar Merah (Flash)
        if (redFlashEffect != null)
        {
            redFlashEffect.Flash();
        }

        // 2. Efek Goyangan Kamera (Shake)
        if (CameraShaker.Instance != null)
        {
            CameraShaker.Instance.Shake(shakeDuration, shakeMagnitude);
        }

        // --- Logic Update UI Health Bar Player ---
        if (playerHealthBarImage != null)
        {
            // Update fillAmount berdasarkan rasio CurrentHealth / maxPlayerHealth
            playerHealthBarImage.fillAmount = CurrentHealth / maxPlayerHealth;
            
            // Atau jika Anda ingin animasi lerp (transisi halus) seperti di EnemyHealth,
            // Anda bisa menggunakan coroutine atau mengupdate di Update().
            // Namun, untuk player health, update langsung biasanya lebih sederhana.
        }
        // ----------------------------------------
        UpdateHealthText();
        
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Game Over! Player Health habis.");
        OnPlayerDefeated?.Invoke();
        // Logika Game Over
    }
}