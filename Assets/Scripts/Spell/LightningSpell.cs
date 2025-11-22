using UnityEngine;
using UnityEngine.Video; // Wajib ada untuk Video
using UnityEngine.UI;    // Wajib ada untuk UI
using System.Collections;

public class LightningSpell : MonoBehaviour
{
    [Header("Settings Stats")]
    [Tooltip("Biaya Mana untuk Spell ini")]
    [SerializeField] private float manaCost = 50f;

    [Tooltip("Besar damage yang diberikan ke SETIAP musuh")]
    [SerializeField] private float damageAmount = 10f;
    
    [Tooltip("Waktu jeda sebelum bisa dipakai lagi")]
    [SerializeField] private float cooldownTime = 15f;

    [Header("Video VFX Settings")]
    [Tooltip("Masukkan GameObject UI (Raw Image) yang ada component VideoPlayer-nya")]
    [SerializeField] private VideoPlayer videoPlayer;

    [Tooltip("Masukkan GameObject RawImage itu lagi di sini (untuk dimunculkan/hilangkan)")]
    [SerializeField] private GameObject videoScreenObj;

    [Tooltip("Berapa detik menunggu video main sebelum damage masuk? (Default 0.5)")]
    [SerializeField] private float delayBeforeDamage = 1f;

    private bool _isOnCooldown = false;

    // Hubungkan fungsi ini ke Tombol UI (On Click)
    public void CastSpell()
    {
        if (_isOnCooldown)
        {
            Debug.Log("Lightning Spell sedang cooldown!");
            return;
        }

        if (PlayerMana.Instance == null || !PlayerMana.Instance.TryConsumeMana(manaCost))
        {
            Debug.Log("Mana tidak cukup!");
            return; 
        }

        StartCoroutine(ProcessLightningSequence());
    }

    private IEnumerator ProcessLightningSequence()
    {
        _isOnCooldown = true;
        Debug.Log("BADAI PETIR DATANG!");

        // --- TAHAP 1: MULAI VIDEO ---
        if (videoPlayer != null && videoScreenObj != null)
        {
            videoScreenObj.SetActive(true); // Munculkan layar hitam/video
            videoPlayer.time = 0;           // Reset video ke detik 0
            videoPlayer.Play();             // Mainkan
        }

        // --- TAHAP 2: TUNGGU MOMEN DAMAGE (0.5 Detik) ---
        yield return new WaitForSeconds(delayBeforeDamage);

        // --- TAHAP 3: LOGIKA DAMAGE ASLIMU ---
        // 1. Cari SEMUA musuh yang aktif
        Enemy[] activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // 2. Loop setiap musuh untuk memberikan damage
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                EnemyHealth healthScript = enemy.GetComponent<EnemyHealth>();
                
                if (healthScript != null)
                {
                    // BERIKAN DAMAGE!
                    healthScript.DealDamage(damageAmount);
                    
                    // Efek visual berkedip pada musuh (tetap kita pakai biar kerasa kena hit)
                    StartCoroutine(FlashEnemy(enemy));
                }
            }
        }
        Debug.Log("Damage diberikan pada detik ke-" + delayBeforeDamage);

        // --- TAHAP 4: TUNGGU VIDEO SELESAI ---
        // Kita hitung sisa waktu video biar layar tidak mati mendadak
        float videoDuration = (float)videoPlayer.length;
        float remainingVideoTime = videoDuration - delayBeforeDamage;

        if (remainingVideoTime > 0)
        {
            yield return new WaitForSeconds(remainingVideoTime);
        }

        // --- TAHAP 5: MATIKAN LAYAR VIDEO ---
        if (videoScreenObj != null)
        {
            videoScreenObj.SetActive(false);
        }

        // --- TAHAP 6: SISA COOLDOWN ---
        // Cooldown dikurangi durasi video yang sudah berjalan
        float remainingCooldown = cooldownTime - videoDuration;
        
        if (remainingCooldown > 0)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }
        
        _isOnCooldown = false;
        Debug.Log("Lightning Spell Siap!");
    }

    // Coroutine kecil untuk bikin musuh berkedip kuning (Visual Feedback)
    private IEnumerator FlashEnemy(Enemy enemy)
    {
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.yellow; // Ubah jadi kuning
            yield return new WaitForSeconds(0.1f); // Kedip sebentar
            
            // Cek lagi apakah musuh masih hidup
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                if (originalColor == Color.cyan) 
                    sr.color = Color.cyan; // Tetap biru jika sedang beku
                else 
                    sr.color = Color.white;
            }
        }
    }
}