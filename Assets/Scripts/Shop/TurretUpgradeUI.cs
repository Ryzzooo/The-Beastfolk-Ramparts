using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurretUpgradeUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button sellButton;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI sellValueText;

    private Node _targetNode;

    // Fungsi ini dipanggil oleh UIManager saat panel dibuka
    public void SetTarget(Node node)
    {
        _targetNode = node; // Simpan referensi Node

        // Ambil info dari turret di node itu untuk update Teks UI
        TurretUpgrade upgradeScript = node.turretOnNode.GetComponent<TurretUpgrade>();

        // Update Teks Jual
        sellValueText.text = "Sell: $" + upgradeScript.GetSellValue();

        // Update Tombol Upgrade
        if (upgradeScript.CanUpgrade())
        {
            upgradeButton.interactable = true;
            upgradeCostText.text = "Upgrade: $" + upgradeScript.GetUpgradeCost();
        }
        else
        {
            upgradeButton.interactable = false;
            upgradeCostText.text = "MAX LEVEL";
        }
    }

    // Hubungkan ini ke OnClick Tombol Upgrade
    public void Upgrade()
    {
        if (_targetNode != null)
        {
            _targetNode.UpgradeTurret();
        }
    }

    // Hubungkan ini ke OnClick Tombol Sell
    public void Sell()
    {
        if (_targetNode != null)
        {
            _targetNode.SellTurret();
        }
    }
}