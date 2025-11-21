using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private float damage = 3f;
    [SerializeField] private float attackSpeed = 1f;
    [SerializeField] private float lifeTime = 10f; // Berapa lama prajurit bertahan sebelum hilang otomatis

    private float _currentHealth;
    private Enemy _targetEnemy; // Musuh yang sedang dihadang
    private bool _isEngaged = false; // Apakah sedang sibuk?

    private void Start()
    {
        _currentHealth = maxHealth;
        
        // Prajurit akan hilang otomatis setelah beberapa detik (biar tidak numpuk)
        Destroy(gameObject, lifeTime); 
    }

    private void Update()
    {
        // Cek jika musuh yang kita pegang tiba-tiba mati (oleh Turret)
        if (_isEngaged && (_targetEnemy == null || !_targetEnemy.gameObject.activeInHierarchy))
        {
            // Kembali siaga
            _isEngaged = false;
            _targetEnemy = null;
            StopAllCoroutines();
        }
    }

    // Deteksi Musuh Lewat
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Sesuatu masuk ke area prajurit: " + other.name);
        // 1. Jika kita SUDAH sibuk, abaikan musuh lain
        if (_isEngaged) return;

        if (other.CompareTag("Enemy"))
        {
            Debug.Log("MUSUH TERDETEKSI! MENYERANG!");
            Enemy enemy = other.GetComponent<Enemy>();
            
            if (enemy != null)
            {
                EngageEnemy(enemy);
            }
        }
    }

    private void EngageEnemy(Enemy enemy)
    {
        _isEngaged = true;
        _targetEnemy = enemy;

        // 1. Beritahu musuh untuk berhenti
        enemy.GetBlocked(this);

        // 2. Mulai kita serang musuh
        StartCoroutine(AttackEnemyRoutine());
    }

    private IEnumerator AttackEnemyRoutine()
    {
        while (_isEngaged && _targetEnemy != null)
        {
            yield return new WaitForSeconds(attackSpeed);

            // Serang Musuh
            if (_targetEnemy != null && _targetEnemy.gameObject.activeInHierarchy)
            {
                // Ambil health musuh
                EnemyHealth enemyHealth = _targetEnemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.DealDamage(damage);
                    // TODO: Animasi prajurit menyerang
                }
            }
        }
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        // Setiap kali prajurit ini nonaktif (mati/durasi habis/game over)
        // Pastikan musuh yang sedang dipegang dilepaskan!
        if (_targetEnemy != null)
        {
            _targetEnemy.ReleaseBlock();
            _targetEnemy = null; // Bersihkan referensi
        }
    }
}