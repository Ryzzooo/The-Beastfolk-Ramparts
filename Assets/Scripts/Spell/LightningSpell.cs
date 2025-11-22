using UnityEngine;
using System.Collections;

public class LightningSpell : MonoBehaviour
{
    [Header("Settings")]

    [Tooltip("Biaya Mana untuk Spell ini")]
    [SerializeField] private float manaCost = 50f;

    [Tooltip("Besar damage yang diberikan ke SETIAP musuh")]
    [SerializeField] private float damageAmount = 10f;
    
    [Tooltip("Waktu jeda sebelum bisa dipakai lagi")]
    [SerializeField] private float cooldownTime = 15f;

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
            return; 
        }

        StartCoroutine(ProcessLightning());
    }

    private IEnumerator ProcessLightning()
    {
        _isOnCooldown = true;
        Debug.Log("BADAI PETIR DATANG!");

        // 1. Cari SEMUA musuh yang aktif di scene saat ini
        Enemy[] activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // 2. Loop setiap musuh untuk memberikan damage
        foreach (Enemy enemy in activeEnemies)
        {
            // Pastikan musuh masih aktif (jaga-jaga)
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                // Ambil komponen Health
                EnemyHealth healthScript = enemy.GetComponent<EnemyHealth>();
                
                if (healthScript != null)
                {
                    // BERIKAN DAMAGE!
                    healthScript.DealDamage(damageAmount);
                    
                    // (Opsional) Efek visual sederhana: Kilatan Kuning
                    StartCoroutine(FlashEnemy(enemy));
                }
            }
        }

        // 3. Tunggu Cooldown
        yield return new WaitForSeconds(cooldownTime);
        
        _isOnCooldown = false;
        Debug.Log("Lightning Spell Siap!");
    }

    // Coroutine kecil untuk bikin musuh berkedip kuning (Visual Feedback)
    // Ini sementara sebelum kamu punya animasi petir sungguhan
    private IEnumerator FlashEnemy(Enemy enemy)
    {
        SpriteRenderer sr = enemy.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            Color originalColor = sr.color;
            sr.color = Color.yellow; // Ubah jadi kuning
            yield return new WaitForSeconds(0.1f); // Kedip sebentar
            
            // Cek lagi apakah musuh masih hidup sebelum kembalikan warna
            // (karena bisa saja dia mati kena damage tadi)
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                // Kembalikan warna (Hati-hati kalau sedang freeze/biru)
                // Logika ini bisa kamu sesuaikan nanti
                if (originalColor == Color.cyan) 
                    sr.color = Color.cyan; // Tetap biru jika sedang beku
                else 
                    sr.color = Color.white;
            }
        }
    }
}