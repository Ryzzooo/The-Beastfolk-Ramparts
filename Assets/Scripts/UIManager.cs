using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Header("Panel References")]
    [Tooltip("Drag panel UI shop kamu ke sini")]
    [SerializeField] private GameObject turretShopPanel;
    
    // (Akan ada panel lain di sini nanti, misal: GameOverPanel)

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