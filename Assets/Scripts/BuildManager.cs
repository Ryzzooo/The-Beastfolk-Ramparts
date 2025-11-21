using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{
    // Menyimpan node yang tadi diklik player
    private Node _selectedNode;

    // Metode ini dipanggil oleh Node.cs (Langkah 1)
    public void SetSelectedNode(Node node)
    {
        // 1. Jika kita sudah memilih node sebelumnya,
        //    matikan highlight node YANG LAMA.
        if (_selectedNode != null)
        {
            _selectedNode.DeselectNode();
        }

        // 2. Simpan node YANG BARU
        _selectedNode = node;

        // 3. Nyalakan highlight node YANG BARU
        _selectedNode.SelectNode();
    }

    // Metode ini akan dipanggil oleh Tombol Shop (Langkah 3)
    public void BuildTurretOnSelectedNode(TurretSettings turretToBuild)
    {
        // 1. Cek apakah kita punya node yang dipilih
        if (_selectedNode == null)
        {
            Debug.LogError("Node belum dipilih!");
            return;
        }

        // 2. Cek apakah node itu masih kosong
        if (_selectedNode.turretOnNode != null)
        {
            Debug.Log("Node sudah terisi!");
            return;
        }

        // 3. Cek apakah uang cukup
        if (CurrencySystem.Instance.TotalCoins < turretToBuild.TurretShopCost)
        {
            Debug.Log("Uang tidak cukup!");
            return;
        }

        // 4. UANG CUKUP & NODE KOSONG -> BELI!
        CurrencySystem.Instance.RemoveCoins(turretToBuild.TurretShopCost);

        // 5. Bangun turret di posisi node
        GameObject turretGO = Instantiate(
            turretToBuild.TurretPrefab, 
            _selectedNode.transform.position, 
            Quaternion.identity
        );

        // 6. Beri tahu node bahwa dia sudah terisi
        _selectedNode.turretOnNode = turretGO.GetComponent<Turret>();

        // 7. (PENTING) Beri tahu TurretUpgrade.cs setting-nya
        TurretUpgrade upgradeScript = turretGO.GetComponent<TurretUpgrade>();
        if (upgradeScript != null)
        {
            upgradeScript.SetTurSettings(turretToBuild);
        }

        // 8. Tutup shop setelah berhasil
        UIManager.Instance.CloseTurretShopPanel();
        _selectedNode = null; // Bersihkan pilihan
    }

    public void ClearSelectedNode()
    {
        if (_selectedNode != null)
        {
            _selectedNode.DeselectNode();
            _selectedNode = null;
        }
    }
}