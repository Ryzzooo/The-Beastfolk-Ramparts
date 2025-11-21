using UnityEngine;

// Perhatikan: Kita mewarisi dari 'Projectile', bukan 'MonoBehaviour'
public class AOEProjectile : Projectile 
{
    [Header("AOE Settings")]
    [Tooltip("Seberapa luas ledakannya")]
    [SerializeField] private float explosionRadius = 3f;
    
    [Tooltip("Layer apa saja yang kena damage (pilih layer Enemy)")]
    [SerializeField] private LayerMask enemyLayer; 
    
    // Kita TIMPA (Override) logika HitTarget milik Projectile biasa
    protected override void HitTarget()
    {
        // 1. Jangan berikan damage ke 1 target saja (hapus logika base)
        // base.HitTarget(); <-- Jangan panggil ini

        // 2. Cari SEMUA collider musuh di dalam lingkaran radius
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        bool hitAnyone = false;

        // 3. Loop semua objek yang kena lingkaran
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            // Cek apakah dia benar-benar musuh (Pakai Tag)
            if (enemyCollider.CompareTag("Enemy"))
            {
                EnemyHealth enemyHealth = enemyCollider.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    // Berikan damage (variabel 'damage' ini ada di script Projectile induk)
                    enemyHealth.DealDamage(damage);
                    hitAnyone = true;
                }
            }
        }

        if (hitAnyone)
        {
            // TODO: Tambahkan Efek Visual Ledakan (Particle System) disini nanti
            Debug.Log("BOOM! Area Damage diberikan.");
        }

        // 4. Kembalikan peluru ke pool (PENTING)
        // Gunakan gameObject.SetActive(false) atau fungsi pooler kamu
        // Karena ini turunan Projectile, kita bisa akses gameObject langsung
        gameObject.SetActive(false); 
    }

    // Fitur Tambahan: Gambar lingkaran di Scene Editor biar gampang atur radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}