using UnityEngine;

public class ReinforcementSpell : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Biaya Mana untuk Spell ini")]
    [SerializeField] private float manaCost = 20f;
    [SerializeField] private float cooldownTime = 20f;

    [Header("Soldier Prefabs (Fixed 2 Units)")]
    [Tooltip("Prajurit yang muncul di Kiri")]
    [SerializeField] private GameObject soldierPrefab1; 
    
    [Tooltip("Prajurit yang muncul di Kanan")]
    [SerializeField] private GameObject soldierPrefab2;

    [Header("Visuals")]
    [SerializeField] private GameObject rangePreview; 

    [Header("Formation")]
    [SerializeField] private float soldierSpacing = 0.7f;

    private bool _isOnCooldown = false;
    private bool _isPlacementMode = false; 

    private void Start()
    {
        if (rangePreview != null) rangePreview.SetActive(false);
    }

    private void Update()
    {
        if (_isPlacementMode)
        {
            HandlePlacementMode();
        }
    }

    // 1. Dipanggil saat tombol ditekan
    public void ActivatePlacementMode()
    {
        if (_isOnCooldown)
        {
            Debug.Log("Spell sedang Cooldown!");
            return;
        }

        // --- CEK MANA (Tanpa mengurangi dulu) ---
        if (PlayerMana.Instance != null)
        {
            if (PlayerMana.Instance.CurrentMana < manaCost)
            {
                Debug.Log("Mana tidak cukup untuk memanggil bala bantuan.");
                return;
            }
        }
        else
        {
            Debug.LogError("PlayerMana instance tidak ditemukan di Scene!");
            return;
        }

        if (_isPlacementMode)
        {
            CancelPlacement();
            return;
        }

        _isPlacementMode = true;
        if (rangePreview != null) rangePreview.SetActive(true);
    }

    private void HandlePlacementMode()
    {
        Vector3 mousePos = GetMouseWorldPosition();
        if (rangePreview != null)
        {
            rangePreview.transform.position = mousePos;
        }

        // 2. Klik Kiri = DEPLOY
        if (Input.GetMouseButtonDown(0)) 
        {
            // --- KURANGI MANA DISINI ---
            // Kita pakai TryConsumeMana dari script PlayerMana.cs kamu
            // Fungsi ini akan return TRUE jika mana cukup dan sudah dikurangi
            if (PlayerMana.Instance.TryConsumeMana(manaCost))
            {
                SpawnSoldiers(mousePos);
                _isPlacementMode = false;
                if (rangePreview != null) rangePreview.SetActive(false);
            }
            else
            {
                Debug.Log("Gagal deploy: Mana tiba-tiba tidak cukup.");
                CancelPlacement();
            }
        }

        // 3. Klik Kanan = CANCEL
        if (Input.GetMouseButtonDown(1)) 
        {
            CancelPlacement();
        }
    }

    private void CancelPlacement()
    {
        _isPlacementMode = false;
        if (rangePreview != null) rangePreview.SetActive(false);
    }

    private void SpawnSoldiers(Vector3 centerPos)
    {
        // Spawn Prajurit 1 (Kiri)
        if (soldierPrefab1 != null)
        {
            Vector3 pos1 = centerPos;
            pos1.x -= soldierSpacing / 2f; 
            Instantiate(soldierPrefab1, pos1, Quaternion.identity);
        }

        // Spawn Prajurit 2 (Kanan)
        if (soldierPrefab2 != null)
        {
            Vector3 pos2 = centerPos;
            pos2.x += soldierSpacing / 2f;
            Instantiate(soldierPrefab2, pos2, Quaternion.identity);
        }

        Debug.Log("2 Prajurit Dikerahkan!");
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
        // Logic UI cooldown bisa ditaruh disini
        yield return new WaitForSeconds(cooldownTime);
        _isOnCooldown = false;
        Debug.Log("Reinforcement Siap Kembali!");
    }
}