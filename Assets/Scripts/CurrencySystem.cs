using UnityEngine;

// Kita akan gunakan pola Singleton yang sama dengan LevelManager-mu
public class CurrencySystem : Singleton<CurrencySystem>
{
    [SerializeField] private int startingCoins = 100;
    
    public int TotalCoins { get; private set; }

    void Start()
    {
        TotalCoins = startingCoins;
        // TODO: Nanti kita update UI koin di sini
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        Debug.Log("Koin ditambahkan: " + amount + ". Total: " + TotalCoins);
        // TODO: Nanti kita update UI koin di sini
    }

    public void RemoveCoins(int amount)
    {
        TotalCoins -= amount;
        Debug.Log("Koin dikurangi: " + amount + ". Total: " + TotalCoins);
        // TODO: Nanti kita update UI koin di sini
    }
}