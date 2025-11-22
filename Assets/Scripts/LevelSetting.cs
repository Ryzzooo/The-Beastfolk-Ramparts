using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewLevel", menuName = "Tower Defense/Level Settings")]
public class LevelSetting : ScriptableObject
{
    public List<Wave> Waves; // Daftar gelombang dalam satu level
}

[System.Serializable]
public class Wave
{
    [Header("Wave Settings")]
    public float TimeBeforeWave; // Jeda waktu sebelum wave ini mulai (misal untuk persiapan)
    public List<EnemyGroup> EnemyGroups; // Dalam 1 wave, bisa ada beberapa jenis musuh
}

[System.Serializable]
public class EnemyGroup
{
    public GameObject EnemyPrefab; // Musuh apa yang mau dikeluarkan? (Rino/Trunk/dll)
    public int EnemyCount;         // Berapa banyak?
    public float SpawnInterval;    // Jeda antar musuh dalam grup ini
}