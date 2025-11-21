using UnityEngine;
using System.Collections;

public class FreezeSpell : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Berapa lama musuh akan beku (detik)")]
    [SerializeField] private float freezeDuration = 3f;
    
    [Tooltip("Cooldown agar tidak bisa spam tombol")]
    [SerializeField] private float cooldownTime = 10f;

    private bool _isOnCooldown = false;

    // Fungsi ini akan dipanggil saat tombol ditekan
    public void CastSpell()
    {
        if (_isOnCooldown)
        {
            Debug.Log("Spell sedang cooldown!");
            return;
        }

        StartCoroutine(FreezeProcess());
    }

    private IEnumerator FreezeProcess()
    {
        // 1. Mulai Cooldown
        _isOnCooldown = true;
        Debug.Log("Mantra Es Dikeluarkan!");

        // 2. Cari SEMUA musuh yang aktif di scene saat ini
        // (Ini metode 'Snapshot' yang kita bicarakan)
        Enemy[] activeEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);

        // 3. Bekukan mereka satu per satu
        foreach (Enemy enemy in activeEnemies)
        {
            if (enemy.gameObject.activeInHierarchy)
            {
                enemy.FreezeMovement();
            }
        }

        // 4. Tunggu selama durasi freeze
        yield return new WaitForSeconds(freezeDuration);

        // 5. Cairkan mereka kembali
        foreach (Enemy enemy in activeEnemies)
        {
            // Cek apakah musuh masih hidup/aktif (mungkin dia mati saat beku ditembak turret)
            if (enemy != null && enemy.gameObject.activeInHierarchy)
            {
                enemy.UnfreezeMovement();
            }
        }

        Debug.Log("Efek Es Hilang.");

        // 6. Tunggu sisa cooldown (opsional, di sini cooldown total = freezeDuration + sisa)
        yield return new WaitForSeconds(cooldownTime - freezeDuration);
        
        _isOnCooldown = false;
        Debug.Log("Spell Siap digunakan lagi!");
    }
}