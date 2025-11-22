using UnityEngine;
using TMPro; // JANGAN LUPA INI!

// Jadikan Singleton (warisi dari base class Singleton Anda)
public class DamageTextManager : Singleton<DamageTextManager>
{
    private ObjectPooler _pooler;
    public ObjectPooler Pooler => _pooler;

    protected override void Awake()
    {
        base.Awake();
        _pooler = GetComponent<ObjectPooler>();
    }

    // Fungsi ini yang akan kita panggil
    public void ShowDamageText(float damageAmount, Vector3 spawnPosition)
    {
        GameObject textObj = _pooler.GetInstanceFromPool();
        
        textObj.transform.position = spawnPosition + new Vector3(0, 0.5f, 0);
        textObj.transform.rotation = Quaternion.identity;

        // --- BAGIAN PENTING ---
        // Ambil skrip dari objek yang baru di-spawn
        DamageText dmgTextScript = textObj.GetComponent<DamageText>();
        
        // Pastikan skripnya ada, lalu set angkanya
        if (dmgTextScript != null)
        {
            dmgTextScript.SetDamage(damageAmount); 
        }
        // ----------------------

        textObj.SetActive(true);
    }
}