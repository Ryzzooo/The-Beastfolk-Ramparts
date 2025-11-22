using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Spawner : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private LevelSetting levelSettings; // File data level (ScriptableObject)
    [SerializeField] private Waypoint path; // Jalur jalan musuh

    // Dictionary untuk menyimpan banyak pool (Pool-Rino, Pool-Trunk, dll)
    private Dictionary<string, ObjectPooler> _poolerDictionary;

    [Header("UI References")]
    [SerializeField] private Button startWaveButton;
    [SerializeField] private TextMeshProUGUI CurrentWaveText;

    private int _currentWaveIndex = 0;
    private int _enemiesAlive;

    public static event Action OnWaveCompleted;
    public static event Action OnLevelCompleted;

    private void Start()
    {
        SetupPoolers();

        // 1. Hubungkan tombol ke fungsi (tapi jangan tampilkan dulu)
        startWaveButton.onClick.AddListener(StartNextWave);

        // 2. Sembunyikan tombol di awal agar rapi
        startWaveButton.gameObject.SetActive(false);

        // 3. LANGSUNG JALANKAN WAVE PERTAMA SECARA OTOMATIS
        StartNextWave();
    }
    public void StartNextWave()
    {
        if (_currentWaveIndex < levelSettings.Waves.Count)
        {
            // Pastikan tombol sembunyi saat wave berjalan
            startWaveButton.gameObject.SetActive(false);
            if (CurrentWaveText != null)
            {
                CurrentWaveText.text = (_currentWaveIndex + 1).ToString();
            }
            Wave currentWave = levelSettings.Waves[_currentWaveIndex];
            CalculateWaveEnemies(currentWave);
            StartCoroutine(SpawnWaveCoroutine(currentWave));
        }
    }
    private void CalculateWaveEnemies(Wave wave)
    {
        _enemiesAlive = 0;
        foreach (var group in wave.EnemyGroups)
        {
            _enemiesAlive += group.EnemyCount;
        }
    }
    private IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        yield return new WaitForSeconds(wave.TimeBeforeWave);

        foreach (EnemyGroup group in wave.EnemyGroups)
        {
            for (int i = 0; i < group.EnemyCount; i++)
            {
                SpawnEnemy(group.EnemyPrefab);
                yield return new WaitForSeconds(group.SpawnInterval);
            }
        }
    }
    private void SetupPoolers()
    {
        _poolerDictionary = new Dictionary<string, ObjectPooler>();

        // Cari semua komponen ObjectPooler di anak-anak GameObject Spawner
        ObjectPooler[] poolers = GetComponentsInChildren<ObjectPooler>();

        foreach (ObjectPooler pooler in poolers)
        {
            // Kita gunakan nama GameObject poolernya sebagai Kunci (Key)
            // Pastikan nama GameObject poolernya mengandung nama musuh (misal "Pool - Rino")
            _poolerDictionary.Add(pooler.gameObject.name, pooler);
        }
    }

    private void SpawnEnemy(GameObject enemyPrefab)
    {
        // Cari Pooler yang tepat berdasarkan nama Prefab
        ObjectPooler poolerToUse = FindPoolerFor(enemyPrefab);

        if (poolerToUse != null)
        {
            GameObject newInstance = poolerToUse.GetInstanceFromPool();

            // --- LOGIKA DARI KODEMU YANG BENAR ---

            // 1. Ambil skrip Enemy
            Enemy enemyScript = newInstance.GetComponent<Enemy>();

            // 2. Set posisi ke titik awal Waypoint
            newInstance.transform.position = path.GetWayPointPosition(0);

            // 3. Set jalur (Path) ke musuh
            enemyScript.SetPath(path); // Asumsi namamu SetPath atau SetWaypoint

            // 4. Aktifkan
            newInstance.SetActive(true);

            // -------------------------------------
        }
        else
        {
            Debug.LogError($"Tidak ada Object Pooler untuk musuh: {enemyPrefab.name}. Pastikan nama GameObject Pool mengandung nama Prefab.");
        }
    }

    private ObjectPooler FindPoolerFor(GameObject prefab)
    {
        ObjectPooler[] allPoolers = GetComponentsInChildren<ObjectPooler>();
        foreach(var pool in allPoolers)
        {
            if (pool.name.Contains(prefab.name)) 
            {
                return pool;
            }
        }
        return null;
    }
    private void OnEnable()
    {
        Enemy.OnEndReached += HandleEnemyGone;
        EnemyHealth.OnEnemyKilled += HandleEnemyGone;
    }

    private void OnDisable()
    {
        Enemy.OnEndReached -= HandleEnemyGone;
        EnemyHealth.OnEnemyKilled -= HandleEnemyGone;
    }

    private void HandleEnemyGone(Enemy enemy)
    {
        _enemiesAlive--;
        if (_enemiesAlive <= 0)
        {
            FinishWave();
        }
    }
    private void FinishWave()
    {

        Debug.Log($"Wave {_currentWaveIndex + 1} Selesai!");
        OnWaveCompleted?.Invoke();

        _currentWaveIndex++;
        if (_currentWaveIndex >= levelSettings.Waves.Count)
        {
            Debug.Log("LEVEL COMPLETED! MENANG!");
            OnLevelCompleted?.Invoke();
        }
        else
        {
            // --- INI PERUBAHANNYA ---
            // Karena ini BUKAN awal game (tapi akhir wave 1),
            // Kita TAMPILKAN tombol agar pemain bisa lanjut ke Wave 2
            // kapanpun mereka siap.
            startWaveButton.gameObject.SetActive(true);
        }
    }
}