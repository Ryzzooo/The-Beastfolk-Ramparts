using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Attributes")]
    [Tooltip("Kecepatan gerak proyektil")]
    [SerializeField] protected float moveSpeed = 10f;
    [Tooltip("Jumlah kerusakan yang diberikan")]
    [SerializeField] protected float damage = 2f;
    
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
        if (transform.parent != null)
        {
            transform.rotation = initialSpawnPoint.rotation;
            _target = null;
            return;
        }

        if (_target == null || !_target.gameObject.activeInHierarchy)
        {
            ObjectPooler.ReturnToPool(gameObject);
            return;
        }

        // Jika kita punya target, bergerak dan berputar
        MoveProjectile();
        RotateProjectile();
    }

    // Metode untuk menggerakkan proyektil
    protected virtual void MoveProjectile()
    {
        // Bergerak lurus ke arah posisi target
        transform.position = Vector2.MoveTowards(
            transform.position, 
            _target.transform.position, 
            moveSpeed * Time.deltaTime
        );

        // Hitung sisa jarak ke target
        float distanceToTarget = (transform.position - _target.transform.position).magnitude;

        // Jika sudah sangat dekat, anggap sudah 'kena'
        if (distanceToTarget < minDistanceToDealDamage)
        {
            HitTarget();
        }
    }

    // Metode saat proyektil mengenai target
    protected virtual void HitTarget()
    {
        if (_target == null) return; // Pengaman ganda

        // Ambil komponen EnemyHealth dari target
        EnemyHealth targetHealth = _target.GetComponent<EnemyHealth>();
        
        // Berikan damage ke target
        if (targetHealth != null)
        {
            // INI PENTING: Memanggil metode dari EnemyHealth.cs
            targetHealth.DealDamage(damage);
        }

        // INI PENTING: Kembalikan proyektil ini ke pool
        ObjectPooler.ReturnToPool(gameObject);
    }

    // Metode untuk memutar proyektil agar menghadap target
    protected virtual void RotateProjectile()
{
    if (_target == null) return; 

    Vector2 direction = _target.transform.position - transform.position;
    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

    // --- UBAH BARIS INI ---
    // transform.rotation = Quaternion.Euler(0, 0, angle); // <--- INI VERSI LAMA
    transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // <--- INI VERSI BARU
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