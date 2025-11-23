using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Attributes")]
    [Tooltip("Kecepatan gerak proyektil")]
    [SerializeField] protected float moveSpeed = 10f;
    [Tooltip("Jumlah kerusakan yang diberikan")]
    [SerializeField] protected float damage = 2f;

    [Tooltip("Putar ini jika panah menghadap arah yang salah. Coba -90, 90, atau 180.")]
    [SerializeField] private float rotationOffset = 0f;
    
    [Header("Precision")]
    [Tooltip("Jarak minimum ke target sebelum proyektil dianggap 'kena'")]
    [SerializeField] private float minDistanceToDealDamage = 0.1f;

    [SerializeField] private Transform initialSpawnPoint;

    // Properti ini ada di gambarmu, untuk menyimpan referensi ke turret
    // yang menembakkannya (berguna untuk upgrade nanti).
    public Turret TurretOwner { get; set; }

    protected Enemy _target;

    // Update dipanggil setiap frame
    protected virtual void Update()
    {
        // 1. Cek awal
        if (_target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        // 2. Gerakkan Peluru
        MoveProjectile();
        
        // --- TAMBAHAN PENGAMAN ---
        // Jika setelah bergerak peluru jadi non-aktif (karena kena target),
        // atau target tiba-tiba hilang (null), BERHENTI DI SINI.
        // Jangan lanjut ke RotateProjectile.
        if (!gameObject.activeSelf || _target == null) return;
        // -------------------------

        // 3. Rotasi Peluru
        RotateProjectile();
    }

    // Metode untuk menggerakkan proyektil
    protected virtual void MoveProjectile()
    {
        transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, moveSpeed * Time.deltaTime);

        // Cek jika sudah kena
        if (Vector2.Distance(transform.position, _target.transform.position) < 0.1f)
        {
            HitTarget();
        }
    }

    // Metode saat proyektil mengenai target
    private void RotateProjectile()
    {
        // Arah dari Panah ke Musuh
        Vector2 direction = _target.transform.position - transform.position;

        // CARA SIMPLE: Paksa sisi "Kanan" (Sumbu Merah) panah menghadap ke musuh
        transform.right = direction;

        // PENTING:
        // Jika setelah pakai kode ini panahmu malah menghadap ke ATAS/BAWAH:
        // Jangan ubah kodingannya.
        // Buka Prefab -> Klik Child Sprite -> Putar Rotation Z Sprite-nya 
        // sampai gambarnya menghadap ke KANAN (Sesuai panah merah Unity).
    }

    // Metode untuk memutar proyektil agar menghadap target
    protected virtual void HitTarget()
    {
        // Berikan damage
        EnemyHealth enemyHealth = _target.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.DealDamage(damage);
        }

        // Kembalikan ke pool
        gameObject.SetActive(false);
    }

    // Ini adalah metode KUNCI yang dipanggil oleh Turret.cs
    // untuk memberi tahu proyektil siapa yang harus dikejar.
    public virtual void SetTarget(Enemy enemy)
    {
        _target = enemy;
    }

    public void SetInitialSpawnPoint(Transform spawnPoint)
    {
        initialSpawnPoint = spawnPoint;
    }

    // PENTING: Saat proyektil kembali ke pool (dinonaktifkan),
    // kita bersihkan referensi targetnya.
    private void OnDisable()
    {
        _target = null;
    }
}