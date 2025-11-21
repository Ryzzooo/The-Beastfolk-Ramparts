using UnityEngine;

public class TurretUpgrade : MonoBehaviour
{
    // Kita akan simpan data turretnya di sini
    private TurretSettings _turretSettings;
    
    // TODO: Tambahkan logika untuk level upgrade nanti

    // Dipanggil oleh BuildManager saat turret dibangun
    public void SetTurSettings(TurretSettings settings)
    {
        _turretSettings = settings;
    }

    // Metode ini dipanggil oleh Node.cs
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