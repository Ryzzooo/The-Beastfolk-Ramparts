using UnityEngine;
using System; // Diperlukan untuk Action

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private int lives = 10;
    
    public int TotalLives { get; set; }
    public int CurrentWave { get; set; }

    private void Start()
    {
        TotalLives = lives;
        CurrentWave = 1;
    }

    // Metode ini akan dipanggil oleh event Enemy.OnEndReached
    private void ReduceLives(Enemy enemy)
    {
        TotalLives--;
        if (TotalLives <= 0)
        {
            TotalLives = 0;
            GameOver();
        }
    }

    private void GameOver()
    {
        // TODO: Tambahkan logika kekalahan di sini
        // Misalnya: Time.timeScale = 0; (pause game)
        //          Tampilkan UI Game Over
        Debug.Log("GAME OVER");
    }

    private void WaveCompleted()
    {
        // TODO: Tambahkan logika menang wave di sini
        // Misalnya: Berikan Gold, Tampilkan UI, hitung mundur wave berikutnya
        Debug.Log("Wave Selesai!");
        CurrentWave++;
    }

    private void OnEnable()
    {
        // Langganan event dari Enemy.cs
        Enemy.OnEndReached += ReduceLives;
        
        // Baris di bawah ini ada di gambarmu, tapi masih comment. 
        // Ini adalah langkah kita selanjutnya
        // Spawner.OnWaveCompleted += WaveCompleted;
    }

    private void OnDisable()
    {
        // Selalu unsubscribe saat object mati/nonaktif
        Enemy.OnEndReached -= ReduceLives;
        // Spawner.OnWaveCompleted -= WaveCompleted;
    }
}