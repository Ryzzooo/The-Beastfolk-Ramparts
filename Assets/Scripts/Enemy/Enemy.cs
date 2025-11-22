using UnityEngine;
using System;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 2f;
    [SerializeField] private float damageToSoldier = 2f; // Damage ke prajurit
    [SerializeField] private float attackSpeed = 1f;
    private float _originalSpeed;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private EnemyHealth _enemyHealth;
    private Vector3 _lastPointPosition;
    private int _currentWaypointIndex;
    private Waypoint _pathWaypoint;
    public Vector3 CurrentPointPosition
    {
        get 
        {
            if (_pathWaypoint != null && _pathWaypoint.Points.Length > 0)
            {
                // Gunakan metode yang sudah didefinisikan di Waypoint.cs
                return _pathWaypoint.GetWayPointPosition(_currentWaypointIndex); 
            }
            return transform.position;
        }
    }
    private bool _isBlocked = false; // Apakah sedang ditahan prajurit?
    private Soldier _blockingSoldier;
    public static Action<Enemy> OnEndReached;

    private void OnEnable()
    {
        // Dapatkan referensi jika belum ada
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_enemyHealth == null)
            _enemyHealth = GetComponent<EnemyHealth>();
        
        // Reset status posisi path
        _currentWaypointIndex = 0;
        
        // Reset kecepatan
        _originalSpeed = MoveSpeed;
        _animator.speed = 1f;

        // --- BAGIAN INI YANG HILANG TADI ---
        // Wajib reset status blokir agar musuh baru tidak diam
        _isBlocked = false;      
        _blockingSoldier = null; 
        
        // Pastikan tidak ada coroutine serangan dari kehidupan sebelumnya yang nyangkut
        StopAllCoroutines();     
    }

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (_isBlocked) 
        {
            return; 
        }

        Move();
        Rotate();

        if (CurrentPointPositionReached())
        {
            UpdateCurretnPointIndex();
        }
    }

    public void GetBlocked(Soldier soldier)
    {
        _isBlocked = true;
        _blockingSoldier = soldier;
        
        // Mulai serang prajurit
        StartCoroutine(AttackSoldierRoutine());
    }

    public void ReleaseBlock()
    {
        _isBlocked = false;
        _blockingSoldier = null;
        StopAllCoroutines(); // Berhenti menyerang
    }

    private IEnumerator AttackSoldierRoutine()
    {
        while (_isBlocked && _blockingSoldier != null)
        {
            // Tunggu sesuai attack speed
            yield return new WaitForSeconds(attackSpeed);

            // Serang Prajurit
            if (_blockingSoldier != null && _blockingSoldier.gameObject.activeInHierarchy)
            {
                _blockingSoldier.TakeDamage(damageToSoldier);
                // TODO: Mainkan animasi serang di sini
            }
        }
    }

    public void SetPath(Waypoint path)
    {
        _pathWaypoint = path;
        _currentWaypointIndex = 0;
        
        // Ini penting: set posisi awal untuk logika Rotate()
        // Kita berasumsi Spawner sudah mengatur posisi transform.position
        _lastPointPosition = transform.position; 
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, CurrentPointPosition, MoveSpeed * Time.deltaTime);
    }

    private void Rotate()
    {
        if (CurrentPointPosition.x > _lastPointPosition.x)
        {
            _spriteRenderer.flipX = false;
        }
        else
        {
            _spriteRenderer.flipX = true;
        }
    }

    private bool CurrentPointPositionReached()
    {
        float distanceToNextPointPosition = (transform.position - CurrentPointPosition).magnitude;
        if (distanceToNextPointPosition < 0.1f)
        {
            _lastPointPosition = transform.position;
            return true;
        }

        return false;
    }

    private void UpdateCurretnPointIndex()
    {
        if (_pathWaypoint == null) return;
        int lastWaypointIndex = _pathWaypoint.Points.Length - 1;
        if (_currentWaypointIndex < lastWaypointIndex)
        {
            _currentWaypointIndex++;
        }
        else
        {
            EndPointReached();
        }
    }

    private void EndPointReached()
    {
        OnEndReached?.Invoke(this);
        _enemyHealth.ResetHealth();
        ObjectPooler.ReturnToPool(gameObject);
    }

    public void FreezeMovement()
    {
        // Simpan kecepatan saat ini (jaga-jaga kalau nanti ada efek slow)
        _originalSpeed = MoveSpeed; 
        
        // Hentikan pergerakan
        MoveSpeed = 0f;
        _animator.speed = 0f;

        // (Opsional) Ubah warna jadi biru agar terlihat beku
        if (_spriteRenderer != null) _spriteRenderer.color = Color.cyan;
    }

    public void UnfreezeMovement()
    {
        // Kembalikan kecepatan
        MoveSpeed = _originalSpeed;
        _animator.speed = 1f;

        // Kembalikan warna
        if (_spriteRenderer != null) _spriteRenderer.color = Color.white;
    }
}
