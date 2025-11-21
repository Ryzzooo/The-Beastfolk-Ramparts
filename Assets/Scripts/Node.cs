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

        // Cek apakah node ini SUDAH ADA isinya
        if (turretOnNode != null)
        {
            // TODO: Buka UI Upgrade/Sell
            Debug.Log("Node ini sudah ada turretnya.");
            return;
        }

        // --- INI BAGIAN PENTING ---
        // Jika kosong, beritahu UIManager untuk membuka shop
        // dan beritahu BuildManager node mana yang dipilih
        
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