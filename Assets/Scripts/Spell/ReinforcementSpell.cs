using UnityEngine;

public class ReinforcementSpell : MonoBehaviour
{
    [Header("Settings")]

    [Tooltip("Biaya Mana untuk Spell ini")]
    [SerializeField] private float manaCost = 20f;
    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private int soldierCount = 2;
    [SerializeField] private float cooldownTime = 20f;
    
    [Header("Visuals")]
    [Tooltip("GameObject visual hantu/lingkaran yang mengikuti mouse (Opsional)")]
    [SerializeField] private GameObject rangePreview; 

    [Header("Formation")]
    [Tooltip("Jarak antar prajurit (semakin kecil semakin dekat)")]
    [SerializeField] private float soldierSpacing = 0.7f;

    private bool _isOnCooldown = false;
    private bool _isPlacementMode = false; // Mode memilih lokasi

    private void Start()
    {
        if (rangePreview != null) rangePreview.SetActive(false);
    }

    private void Update()
    {
        // Hanya jalankan logika ini jika sedang mode menaruh prajurit
        if (_isPlacementMode)
        {
            HandlePlacementMode();
        }
    }

    // Fungsi ini dipanggil tombol UI
    public void ActivatePlacementMode()
    {
        if (_isOnCooldown)
        {
            Debug.Log("Spell sedang Cooldown!");
            return;
        }

        if (PlayerMana.Instance == null || PlayerMana.Instance.CurrentMana < manaCost)
        {
            Debug.Log("Mana tidak cukup untuk memanggil bala bantuan.");
            return;
        }

        if (_isPlacementMode)
        {
            // Jika diklik lagi saat mode aktif, batalkan (Cancel)
            CancelPlacement();
            return;
        }

        _isPlacementMode = true;
        Debug.Log("Pilih lokasi untuk prajurit...");
        
        if (rangePreview != null) rangePreview.SetActive(true);
    }

    private void HandlePlacementMode()
    {
        // 1. Ikuti posisi mouse (untuk preview)
        Vector3 mousePos = GetMouseWorldPosition();
        if (rangePreview != null)
        {
            rangePreview.transform.position = mousePos;
        }

        // 2. Deteksi Klik Kiri (DEPLOY)
        if (Input.GetMouseButtonDown(0)) // 0 = Klik Kiri
        {
            SpawnSoldiers(mousePos);
            _isPlacementMode = false;
            if (rangePreview != null) rangePreview.SetActive(false);
        }

        // 3. Deteksi Klik Kanan (CANCEL)
        if (Input.GetMouseButtonDown(1)) // 1 = Klik Kanan
        {
            CancelPlacement();
        }
    }

    private void CancelPlacement()
    {
        _isPlacementMode = false;
        if (rangePreview != null) rangePreview.SetActive(false);
        Debug.Log("Deployment Dibatalkan.");
    }

    private void SpawnSoldiers(Vector3 centerPos)
    {
        for (int i = 0; i < soldierCount; i++)
        {
            float xOffset = (i - (soldierCount - 1) / 2f) * soldierSpacing;
            float yOffset = 0;
            Vector3 spawnPosition = new Vector3(centerPos.x + xOffset, centerPos.y + yOffset, 0);
            Instantiate(soldierPrefab, spawnPosition, Quaternion.identity);
        }

        Debug.Log("Prajurit Dikerahkan!");
        StartCoroutine(CooldownRoutine());
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    private System.Collections.IEnumerator CooldownRoutine()
    {
        _isOnCooldown = true;
        // TODO: Update UI Cooldown (misal tombol jadi abu-abu) di sini
        yield return new WaitForSeconds(cooldownTime);
        _isOnCooldown = false;
        // TODO: Update UI Cooldown (tombol nyala lagi)
        Debug.Log("Reinforcement Siap!");
    }
}