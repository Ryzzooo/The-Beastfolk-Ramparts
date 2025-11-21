using UnityEngine;
using UnityEngine.UI; // Diperlukan untuk Image dan Button
using TMPro; // Diperlukan untuk TextMeshProUGUI

public class TurretCard : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image turretImage;
    [SerializeField] private TextMeshProUGUI turretCost;

    [Header("Turret Data")]
    [SerializeField] private TurretSettings turretSettings;


    private Button _button;

    private void Awake()
    {
        // Ambil referensi Button dan tambahkan listener
        _button = GetComponent<Button>();
        _button.onClick.AddListener(PlaceTurret);
    }

    // Metode ini dipanggil oleh skrip "TurretShopManager" (atau semacamnya)
    // untuk mengisi data ke kartu ini.
    private void Start()
    {
        // Cek jika kamu lupa mengisi data di Inspector
        if (turretSettings == null)
        {
            Debug.LogError("TurretSettings belum di-set di Inspector!", this);
            gameObject.SetActive(false);
            return;
        }
        
        // Langsung setup UI-nya sendiri
        turretImage.sprite = turretSettings.TurretShopSprite;
        turretCost.text = turretSettings.TurretShopCost.ToString();
    }

    // Metode ini dipanggil saat tombol di-klik
    public void PlaceTurret()
    {
        if (turretSettings == null) return;

        BuildManager.Instance.BuildTurretOnSelectedNode(turretSettings);
    }
}