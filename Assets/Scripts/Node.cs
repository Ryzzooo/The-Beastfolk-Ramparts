using UnityEngine;

public class Node : MonoBehaviour
{
    [Tooltip("Turret yang sedang menempati node ini")]
    public Turret turretOnNode;

    // GameObject untuk highlight (opsional)
    [SerializeField] private GameObject selectionHighlight; 

    private void Start()
    {
        // Pastikan highlight mati saat game dimulai
        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    // Ini akan otomatis dipanggil saat GameObject ini diklik
    private void OnMouseDown()
    {
        Debug.Log("Node diklik!");

        if (turretOnNode != null)
        {
            // Simpan seleksi (agar highlight nyala)
            BuildManager.Instance.SetSelectedNode(this);
            
            // Buka Panel Upgrade
            UIManager.Instance.OpenUpgradePanel(this);
            return;
        }
        
        // (Kita akan buat skrip-skrip ini di langkah berikutnya)
        BuildManager.Instance.SetSelectedNode(this);
        UIManager.Instance.OpenTurretShopPanel();
    }

    // Metode untuk highlight saat mouse di atasnya (opsional)
    private void OnMouseEnter()
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }

    public void UpgradeTurret()
    {
        if (turretOnNode == null) return;

        TurretUpgrade upgradeScript = turretOnNode.GetComponent<TurretUpgrade>();

        if (upgradeScript == null || !upgradeScript.CanUpgrade())
        {
            Debug.Log("Tidak bisa upgrade.");
            return;
        }

        // Bayar & Bangun
        CurrencySystem.Instance.RemoveCoins(upgradeScript.GetUpgradeCost());
        BuildManager.Instance.UpgradeTurretOnNode(this, upgradeScript.GetUpgradeTo());
        
        UIManager.Instance.CloseUpgradePanel();
    }

    // --- FUNGSI SELL (Disini tempatnya!) ---
    public void SellTurret()
    {
        if (turretOnNode == null) return;

        TurretUpgrade upgradeScript = turretOnNode.GetComponent<TurretUpgrade>();
        int sellValue = (upgradeScript != null) ? upgradeScript.GetSellValue() : 0;

        CurrencySystem.Instance.AddCoins(sellValue);
        Destroy(turretOnNode.gameObject);
        turretOnNode = null;
        
        DeselectNode(); // Matikan highlight
        UIManager.Instance.CloseUpgradePanel();
    }

    public void SelectNode()
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(true);
    }

    public void DeselectNode()
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(false);
    }
}