using UnityEngine;
using System.Collections.Generic;
using System.Linq; 
using System.Collections;

public class Turret : MonoBehaviour
{
    [Tooltip("Sprite Karakter (Archer/Mage) untuk di-flip")]
    [SerializeField] private SpriteRenderer characterSprite;
    [Tooltip("Animator Karakter (untuk animasi serangan)")]
    [SerializeField] private Animator characterAnimator;

    [Header("Attributes")]
    [SerializeField] private float attackRange = 4f;

    [Header("Shooting")] // <-- TAMBAHAN BARU
    [SerializeField] private GameObject projectilePrefab; // Prefab proyektil (dari gambarmu)
    [SerializeField] private Transform projectileSpawnPoint; // Titik laras tempat peluru keluar
    [SerializeField] private float attackCooldown = 1f; // Waktu jeda antar tembakan
    [Tooltip("Berapa detik jeda antara animasi mulai sampai peluru keluar? Sesuaikan dengan gerakan tangan archer.")]
    [SerializeField] private float shootDelay = 1f;

    // Variabel dari kodemu
    private List<Enemy> _enemies = new List<Enemy>();

    public TurretUpgrade TurretUpgrade { get; private set; }
    
    private CircleCollider2D _rangeCollider;
    private ObjectPooler _pooler; // <-- TAMBAHAN BARU (Untuk pool proyektil)
    private float _cooldownTimer; // <-- TAMBAHAN BARU (Timer menembak)

    private Enemy _currentEnemyTarget; 
    // Properti publik (read-only)
    public Enemy CurrentEnemyTarget => _currentEnemyTarget;

    private void Awake()
    {
        _rangeCollider = GetComponent<CircleCollider2D>();
        
        TurretUpgrade = GetComponent<TurretUpgrade>();
        if (TurretUpgrade == null)
        {
            Debug.LogError("Prefab Turret ini tidak punya TurretUpgrade.cs!", this);
        }

        // Ambil pooler yang ada di turret ini
        _pooler = GetComponent<ObjectPooler>();
    }

    private void Update()
    {
        GetCurrentEnemyTarget();
        HandleShooting();
        HandleSpriteFlip();   
    }

    private void HandleSpriteFlip()
    {
        // Jika tidak ada target ATAU tidak ada sprite karakter (misal Mortar), jangan lakukan apa-apa
        if (_currentEnemyTarget == null || characterSprite == null) return;

        // Cek posisi X musuh relatif terhadap tower
        bool enemyIsOnRight = _currentEnemyTarget.transform.position.x > transform.position.x;

        // Atur flipX berdasarkan posisi musuh
        // Asumsi: Gambar asli Archer menghadap KANAN
        // Jika musuh di Kanan -> FlipX = false (Tetap hadap kanan)
        // Jika musuh di Kiri  -> FlipX = true (Balik hadap kiri)
        characterSprite.flipX = !enemyIsOnRight;
    }

    private void HandleShooting()
    {
        // Jika tidak ada target, jangan lakukan apa-apa
        if (CurrentEnemyTarget == null)
        {
            _cooldownTimer = attackCooldown; // Reset timer jika tidak ada target
            return;
        }

        // Hitung mundur timer
        _cooldownTimer -= Time.deltaTime;

        // Jika timer habis, tembak!
        if (_cooldownTimer <= 0f)
        {
            StartCoroutine(FireRoutine());
            _cooldownTimer = attackCooldown; // Reset timer
        }
    }

    private IEnumerator FireRoutine()
    {
        // 1. MAINKAN ANIMASI DULUAN
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("Shoot");
        }

        // 2. TUNGGU SEBENTAR (Sesuai gerakan menarik busur)
        // Archer menarik tali... tahan...
        yield return new WaitForSeconds(shootDelay);

        // 3. BARU KELUARKAN PELURU (LEPAS!)
        // Cek lagi apakah target masih ada (bisa saja mati saat kita menunggu delay)
        if (CurrentEnemyTarget != null || transform.name.Contains("Mortar")) 
        {
             SpawnProjectile(); // Panggil fungsi spawn peluru
        }
    }

    private void SpawnProjectile()
    {
        GameObject projectileGO = _pooler.GetInstanceFromPool();
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        projectileGO.transform.position = projectileSpawnPoint.position;

        if (CurrentEnemyTarget != null)
        {
            Vector3 direction = CurrentEnemyTarget.transform.position - projectileSpawnPoint.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectileGO.transform.rotation = Quaternion.Euler(0, 0, angle); 
        }
        else
        {
            projectileGO.transform.rotation = projectileSpawnPoint.rotation;
        }

        projectileGO.SetActive(true);
        projectile.SetTarget(CurrentEnemyTarget);
    }

    // --- Sisa skrip (GetCurrentEnemyTarget, RotateTowardsTarget, OnTrigger, dll) ---
    // --- Biarkan apa adanya, tidak perlu diubah ---

    private void GetCurrentEnemyTarget()
    {
        _enemies.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy);
        _currentEnemyTarget = (_enemies.Count > 0) ? _enemies[0] : null;
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy newEnemy = other.GetComponent<Enemy>();
            if (newEnemy != null && !_enemies.Contains(newEnemy))
            {
                _enemies.Add(newEnemy);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null && _enemies.Contains(enemy))
            {
                _enemies.Remove(enemy);
            }
            if (enemy == _currentEnemyTarget)
            {
                _currentEnemyTarget = null;
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}