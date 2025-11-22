using UnityEngine;

public class EndPointTrigger : MonoBehaviour
{
    // Opsi: Referensi langsung ke PlayerHealth
    // Jika hanya ada 1 PlayerHealth di Scene, Anda bisa mencarinya di Awake/Start
    private PlayerHealth _playerHealth;

    [Tooltip("Damage yang diberikan ke Player ketika musuh melewati collider ini.")]
    [SerializeField] private float damageOnPass = 1f;

    void Start()
    {
        // Ganti FindObjectOfType() dengan FindAnyObjectByType()
        _playerHealth = FindAnyObjectByType<PlayerHealth>(); 
        if (_playerHealth == null)
        {
            // ...
        }
    }

    // Dipanggil ketika objek lain dengan Collider2D dan Rigidbody2D melewati Trigger ini
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Cek apakah yang masuk ke dalam trigger adalah musuh
        if (other.CompareTag("Enemy"))
        {
            Enemy enemyComponent = other.GetComponent<Enemy>();
            
            if (enemyComponent != null)
            {
                Debug.Log($"Enemy '{other.gameObject.name}' reached the goal via collider trigger.");

                // 1. Berikan Damage ke Player
                if (_playerHealth != null)
                {
                    _playerHealth.TakeDamage(damageOnPass);
                }

                // 2. Musuh harus dinonaktifkan/dipool
                // Panggil method yang sudah ada di skrip Enemy.cs untuk EndPointReached
                // CATATAN: Pastikan musuh yang masuk collider BUKAN yang sudah dipool oleh Waypoint
                
                // Jika ingin menggunakan logika EndPointReached di Enemy.cs, 
                // Anda harus mengeksposnya menjadi public atau membuat method khusus.
                
                // Simplifikasi: Langsung pool musuh di sini.
                ObjectPooler.ReturnToPool(other.gameObject); 
                
                // Tambahan: Panggil ReleaseBlock jika musuh sedang ditahan agar Soldier tidak error
                enemyComponent.ReleaseBlock();
            }
        }
    }
}