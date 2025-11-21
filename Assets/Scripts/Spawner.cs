using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int enemyCount = 10;

    [Header("Fixed Delay")]
    [SerializeField] private float delayBtwSpawns;

    [Header("References")] // <-- Tambahkan header ini
    [SerializeField] private Waypoint path; // <-- TAMBAHKAN baris ini

    private float _spawnTimer;
    private int _enemiesSpawned;
    private ObjectPooler _pooler;

    private void Start()
    {
        _pooler = GetComponent<ObjectPooler>();

        if (path == null)
        {
            Debug.LogError("Waypoint (Path) belum di-assign di Spawner!", this);
            this.enabled = false; // Matikan spawner jika path tidak ada
        }
    }

    private void Update()
    {
        _spawnTimer -= Time.deltaTime;

        if (_spawnTimer < 0)
        {
            _spawnTimer = delayBtwSpawns;

            if (_enemiesSpawned < enemyCount)
            {
                _enemiesSpawned++;
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        GameObject newInstance = _pooler.GetInstanceFromPool();

        // 1. Dapatkan komponen Enemy
        Enemy enemy = newInstance.GetComponent<Enemy>();

        if (enemy != null)
        {
            // 2. Set posisi musuh ke TITIK AWAL path
            // Kita panggil method dari Waypoint.cs
            newInstance.transform.position = path.GetWayPointPosition(0);

            // 3. Beri tahu musuh path mana yang harus diikuti
            enemy.SetPath(path);
        }
        else
        {
            Debug.LogError("Prefab musuh tidak memiliki komponen Enemy.cs!", newInstance);
        }

        // 4. Baru aktifkan musuh
        newInstance.SetActive(true);
    }
}
