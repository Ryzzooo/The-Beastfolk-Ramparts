using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Panel References")]
    [Tooltip("Drag panel UI shop kamu ke sini")]
    [SerializeField] private GameObject turretShopPanel;

    [SerializeField] private GameObject upgradePanel;
    private TurretUpgradeUI _upgradeUI; // Referensi ke script UI-nya
    
    // (Akan ada panel lain di sini nanti, misal: GameOverPanel)

    private void Awake() // Atau Start
    {
        // Ambil script TurretUpgradeUI dari panelnya
        if (upgradePanel != null)
            _upgradeUI = upgradePanel.GetComponent<TurretUpgradeUI>();
    }

    public void OpenUpgradePanel(Node node)
    {
        // Pastikan Shop tutup dulu biar gak numpuk
        CloseTurretShopPanel();

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);
            
            // Kirim data Node ke script UI agar tombolnya tahu harus upgrade siapa
            if (_upgradeUI != null)
            {
                _upgradeUI.SetTarget(node);
            }
        }
    }

    public void CloseUpgradePanel()
    {
        if (upgradePanel != null)
        {
            upgradePanel.SetActive(false);
            
            // PENTING: Bersihkan seleksi di BuildManager
            BuildManager.Instance.ClearSelectedNode();
        }
    }

    // Metode ini dipanggil oleh TurretCard.cs
    public void CloseTurretShopPanel()
    {
        if (turretShopPanel != null)
        {
            turretShopPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Turret Shop Panel belum di-assign di UIManager!");
        }
        BuildManager.Instance.ClearSelectedNode();
    }
    
    // Kamu juga bisa tambahkan metode untuk membukanya
    public void OpenTurretShopPanel()
    {
        if (turretShopPanel != null)
        {
            turretShopPanel.SetActive(true);
        }
    }
}