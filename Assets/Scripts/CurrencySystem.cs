using TMPro;
using UnityEngine;

// Kita akan gunakan pola Singleton yang sama dengan LevelManager-mu
public class CurrencySystem : Singleton<CurrencySystem>
{
    [SerializeField] private int startingCoins = 100;
    [SerializeField] private TextMeshProUGUI jumlahCoin;
    
    public int TotalCoins { get; private set; }

    void Start()
    {
        TotalCoins = startingCoins;
        jumlahCoin.text = TotalCoins.ToString();
        // TODO: Nanti kita update UI koin di sini
    }

    public void AddCoins(int amount)
    {
        TotalCoins += amount;
        jumlahCoin.text = TotalCoins.ToString();
        Debug.Log("Koin ditambahkan: " + amount + ". Total: " + TotalCoins);
        // TODO: Nanti kita update UI koin di sini
    }

    public void RemoveCoins(int amount)
    {
        TotalCoins -= amount;
        jumlahCoin.text = TotalCoins.ToString();
        Debug.Log("Koin dikurangi: " + amount + ". Total: " + TotalCoins);
        // TODO: Nanti kita update UI koin di sini
    }
}