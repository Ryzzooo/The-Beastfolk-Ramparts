using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

public class Turret : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Bagian turret yang akan berputar (laras)")]
    [SerializeField] private Transform turretModel; 

    [Header("Attributes")]
    [SerializeField] private float attackRange = 4f;
    [SerializeField] private float rotationSpeed = 10f; 

    [Header("Shooting")] // <-- TAMBAHAN BARU
    [SerializeField] private GameObject projectilePrefab; // Prefab proyektil (dari gambarmu)
    [SerializeField] private Transform projectileSpawnPoint; // Titik laras tempat peluru keluar
    [SerializeField] private float attackCooldown = 1f; // Waktu jeda antar tembakan

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
        _rangeCollider.radius = attackRange;
        TurretUpgrade = GetComponent<TurretUpgrade>();
        if (TurretUpgrade == null)
        {
            Debug.LogError("Prefab Turret ini tidak punya TurretUpgrade.cs!", this);
        }

        // Ambil pooler yang ada di turret ini
        _pooler = GetComponent<ObjectPooler>(); 

        if (turretModel == null)
        {
            turretModel = transform;
        }
    }

    private void Update()
    {
        GetCurrentEnemyTarget();
        RotateTowardsTarget();
        HandleShooting();
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
            Fire();
            _cooldownTimer = attackCooldown; // Reset timer
        }
    }

    private void Fire()
    {
        // Ambil proyektil dari pool
        GameObject projectileGO = _pooler.GetInstanceFromPool();

        // Ambil skrip Projectile.cs dari proyektil itu
        Projectile projectile = projectileGO.GetComponent<Projectile>();

        // Set posisi & rotasi proyektil
        projectileGO.transform.position = projectileSpawnPoint.position;
        projectileGO.transform.rotation = projectileSpawnPoint.rotation;
        
        // Ini adalah bagian TERPENTING:
        // Memberi tahu proyektil siapa targetnya
        projectile.SetTarget(CurrentEnemyTarget);

        // Aktifkan proyektil
        projectileGO.SetActive(true);
    }

    // --- Sisa skrip (GetCurrentEnemyTarget, RotateTowardsTarget, OnTrigger, dll) ---
    // --- Biarkan apa adanya, tidak perlu diubah ---

    private void GetCurrentEnemyTarget()
    {
        _enemies.RemoveAll(e => e == null || !e.gameObject.activeInHierarchy);
        _currentEnemyTarget = (_enemies.Count > 0) ? _enemies[0] : null;
    }

    private void RotateTowardsTarget()
    {
        if (_currentEnemyTarget == null) return;
        Vector2 direction = CurrentEnemyTarget.transform.position - turretModel.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        turretModel.rotation = Quaternion.Slerp(turretModel.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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