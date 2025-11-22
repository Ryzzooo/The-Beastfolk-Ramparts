using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro; // Untuk TextMeshPro

// PENTING: Skrip ini bergantung pada EnemyHealth.OnEnemyKilled
// Pastikan EnemyHealth.cs memiliki: public static Action<Enemy> OnEnemyKilled;

public class PlayerMana : MonoBehaviour
{
    // Menggunakan pola Singleton agar mudah diakses dari script Spell
    public static PlayerMana Instance { get; private set; }

    [Header("Mana Stats")]
    [SerializeField] private float maxMana = 100f;
    
    [Tooltip("Jumlah Mana yang didapat setiap kali musuh terbunuh")]
    [SerializeField] private float manaPerKill = 5f; 
    public float CurrentMana { get; private set; }

    [Header("UI Reference")]
    [SerializeField] private Image manaBarImage;
    [SerializeField] private TextMeshProUGUI manaText; 

    void Awake()
    {
        // Inisialisasi Singleton
        if (Instance == null)
        {
            Instance = this;
            CurrentMana = maxMana;
            UpdateManaUI();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    // --- PENANGANAN EVENT KILL ---
    
    private void OnEnable()
    {
        // Berlangganan ke event ketika musuh mati
        // Gunakan try-catch atau pengecekan null untuk event statis
        try
        {
            EnemyHealth.OnEnemyKilled += OnEnemyKilled; 
        }
        catch (NullReferenceException)
        {
            Debug.LogError("PlayerMana: Tidak bisa berlangganan OnEnemyKilled. Pastikan EnemyHealth.cs memiliki deklarasi public static Action<Enemy> OnEnemyKilled;");
        }
    }
    
    private void OnDisable()
    {
        // Berhenti berlangganan saat objek dinonaktifkan
        EnemyHealth.OnEnemyKilled -= OnEnemyKilled;
    }
    
    // Handler yang dipanggil ketika musuh mati
    private void OnEnemyKilled(Enemy enemy)
    {
        // Menggunakan variabel manaPerKill untuk memulihkan Mana
        RestoreMana(manaPerKill); 
        Debug.Log($"Mana bertambah {manaPerKill} karena kill.");
    }
    
    // Hapus atau biarkan kosong: Update() tidak diperlukan untuk regen pasif
    void Update()
    {
        // Kosong. Mana sekarang hanya diregenerasi melalui OnEnemyKilled.
    }

    /// <summary>
    /// Mencoba mengurangi Mana sebesar cost yang diberikan.
    /// </summary>
    /// <param name="cost">Biaya Mana yang diperlukan.</param>
    /// <returns>True jika Mana cukup dan berhasil dikurangi, False jika tidak.</returns>
    public bool TryConsumeMana(float cost)
    {
        if (CurrentMana >= cost)
        {
            CurrentMana -= cost;
            UpdateManaUI();
            return true;
        }
        else
        {
            Debug.Log("Mana tidak cukup untuk menggunakan Spell.");
            return false;
        }
    }
    
    /// <summary>
    /// Menambah Mana ke nilai saat ini.
    /// </summary>
    public void RestoreMana(float amount)
    {
        CurrentMana += amount;
        // Pastikan Mana tidak melebihi nilai maksimum
        CurrentMana = Mathf.Clamp(CurrentMana, 0f, maxMana); 
        UpdateManaUI();
    }
    
    /// <summary>
    /// Memperbarui tampilan Mana Bar dan angka Mana.
    /// </summary>
    private void UpdateManaUI()
    {
        // Update bar UI
        if (manaBarImage != null)
        {
            manaBarImage.fillAmount = CurrentMana / maxMana;
        }
        
        // Update teks UI
        if (manaText != null)
        {
            // Menggunakan FloorToInt agar angka tidak memiliki koma
            int current = Mathf.FloorToInt(CurrentMana);
            int max = Mathf.FloorToInt(maxMana);
            manaText.text = $"{current} / {max}";
        }
    }
}