using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 30f;
    [SerializeField] private float damage = 3f;

    [Header("Attack Settings")]
    [Tooltip("Kapan damage masuk (sesuaikan dengan gerakan tangan soldier)")]
    [SerializeField] private float hitDelay = 0.4f; 
    
    [Tooltip("Berapa lama soldier DIAM setelah memukul")]
    [SerializeField] private float attackCooldown = 1.0f; 

    [SerializeField] private float lifeTime = 15f; 

    [Header("Components")]
    [SerializeField] private Animator animator; 

    private float _currentHealth;
    private Enemy _targetEnemy; 
    private bool _isEngaged = false; 

    private void Start()
    {
        _currentHealth = maxHealth;
        if (animator == null) animator = GetComponent<Animator>();
        Destroy(gameObject, lifeTime); 
    }

    private void Update()
    {
        if (_isEngaged && (_targetEnemy == null || !_targetEnemy.gameObject.activeInHierarchy))
        {
            StopFighting();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isEngaged) return;

        if (other.CompareTag("Enemy"))
        {
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

        enemy.GetBlocked(this);

        StartCoroutine(AttackEnemyRoutine());
    }

    private void StopFighting()
    {
        _isEngaged = false;
        _targetEnemy = null;
        StopAllCoroutines();

        // Pastikan saat berhenti, animasi dipaksa balik ke Idle (False)
        if (animator != null) animator.SetBool("onAttack", false);
    }

    private IEnumerator AttackEnemyRoutine()
    {
        while (_isEngaged && _targetEnemy != null)
        {
            // 1. NYALAKAN Animasi (Mulai mengayun)
            if (animator != null) 
            {
                animator.SetBool("onAttack", true); 
            }

            // 2. Tunggu momen pedang kena
            yield return new WaitForSeconds(hitDelay);

            // 3. Berikan Damage
            if (_targetEnemy != null && _targetEnemy.gameObject.activeInHierarchy)
            {
                EnemyHealth enemyHealth = _targetEnemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.DealDamage(damage);
                }
            }

            // 4. MATIKAN Animasi (PENTING!)
            // Kita ubah jadi FALSE sekarang.
            // Karena "Has Exit Time" di Unity nanti kita nyalakan, 
            // dia akan menyelesaikan ayunan dulu baru balik ke Idle.
            if (animator != null) 
            {
                animator.SetBool("onAttack", false);
            }

            // 5. Tunggu Cooldown (Fase Diam/Idle)
            // Soldier akan berdiri diam selama waktu ini sebelum loop mengulang ke atas
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    public void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        if (_targetEnemy != null)
        {
            _targetEnemy.ReleaseBlock();
            _targetEnemy = null;
        }
    }
}