using UnityEngine;

public class TurretUpgrade : MonoBehaviour
{
    private TurretSettings _turretSettings;
    
    public void SetTurSettings(TurretSettings settings)
    {
        _turretSettings = settings;
    }

    public bool CanUpgrade()
    {
        if (_turretSettings == null) return false;

        // Bisa upgrade JIKA:
        // 1. Ada data upgrade-nya (tidak kosong)
        // 2. Uang player cukup
        return _turretSettings.UpgradeTo != null && 
               CurrencySystem.Instance.TotalCoins >= _turretSettings.UpgradeCost;
    }

    public TurretSettings GetUpgradeTo()
    {
        if (_turretSettings != null) return _turretSettings.UpgradeTo;
        return null;
    }

    public int GetUpgradeCost()
    {
        if (_turretSettings != null) return _turretSettings.UpgradeCost;
        return 0;
    }

    public int GetSellValue()
    {
        if (_turretSettings == null)
        {
            Debug.LogWarning("TurretSettings belum di-set di TurretUpgrade!");
            return 0; // Jika tidak ada data, jual seharga 0
        }

        // Kita jual setengah harga (contoh)
        return Mathf.RoundToInt(_turretSettings.TurretShopCost / 2f);
    }
}