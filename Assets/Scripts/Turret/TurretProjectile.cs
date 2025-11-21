using UnityEngine;

// Nama skrip ini sepertinya TurretProjectile.cs
// Ini bertugas sebagai MEKANISME TEMBAK, dan bekerja 
// bersama skrip Turret.cs (yang berputar & mencari target).
public class TurretProjectile : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Posisi di ujung laras tempat proyektil 'dimuat'")]
    [SerializeField] protected Transform turretProjectileSpawnPosition;
    
    // Referensi ke skrip lain di turret ini
    protected ObjectPooler _pooler; // Pool yang berisi prefab proyektil
    protected Turret _turret;       // Skrip Turret.cs yang berputar dan mencari target

    [Header("Attributes")]
    [Tooltip("Jeda waktu antar tembakan (cooldown)")]
    [SerializeField] protected float delayBtwAttacks = 2f;
    
    // Sebaiknya 'damage' ada di prefab proyektil, tapi kita ikuti gambarmu
    [SerializeField] protected float damage = 2f; 

    // Variabel internal
    protected Projectile _currentProjectileLoaded; // Proyektil yang "siap tembak"
    protected float _nextAttackTime; // Waktu untuk tembakan berikutnya

    private void Start()
    {
        // Ambil referensi skrip lain yang ada di GameObject ini
        _turret = GetComponent<Turret>();
        _pooler = GetComponent<ObjectPooler>();
        
        // Cek error jika ada yang kurang
        if (_pooler == null)
            Debug.LogError("Turret ini butuh komponen ObjectPooler!", this);
        if (_turret == null)
            Debug.LogError("Turret ini butuh komponen Turret.cs!", this);
        if (turretProjectileSpawnPosition == null)
            Debug.LogError("Belum set 'Turret Projectile Spawn Position'!", this);
    }

    private void Update()
    {
        // Jika turret "kosong" (belum ada proyektil di laras)
        if (IsTurretEmpty())
        {
            loadProjectile(); // Muat proyektil baru
        }

        // Cek jika sudah waktunya menembak
        if (Time.time > _nextAttackTime)
        {
            // Cek jika ada target (dari skrip Turret.cs) DAN proyektil sudah dimuat
            if (_turret.CurrentEnemyTarget != null && _currentProjectileLoaded != null)
            {
                FireProjectile();
            }
        }
    }

    private void FireProjectile()
    {
        // 1. "TEMBAK!" - Lepaskan proyektil dari laras (un-parent)
        _currentProjectileLoaded.transform.parent = null;
        
        // 2. Beri tahu proyektil (dari skrip Projectile.cs) siapa targetnya
        _currentProjectileLoaded.SetTarget(_turret.CurrentEnemyTarget);

        // 3. Set proyektil yang "dimuat" jadi null, agar turret bisa reload
        _currentProjectileLoaded = null;

        // 4. Atur cooldown untuk tembakan berikutnya
        _nextAttackTime = Time.time + delayBtwAttacks;
    }

    // Memuat proyektil baru dari pool dan menempelkannya ke laras
    protected virtual void loadProjectile()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool();
        if (newInstance == null) return; 

        newInstance.transform.position = turretProjectileSpawnPosition.position;
        newInstance.transform.SetParent(turretProjectileSpawnPosition);

        _currentProjectileLoaded = newInstance.GetComponent<Projectile>();
        
        // --- TAMBAHKAN BARIS INI ---
        // Memberi tahu proyektil siapa spawn point aslinya
        _currentProjectileLoaded.SetInitialSpawnPoint(turretProjectileSpawnPosition);
        // --- AKHIR TAMBAHAN ---

        newInstance.SetActive(true);
    }

    // Cek apakah turret sedang "kosong" (tidak ada proyektil di laras)
    public bool IsTurretEmpty()
    {
        return _currentProjectileLoaded == null;
    }
}