using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour
{
    [Tooltip("Prefab Musuh yang Akan di-Pool")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int poolSize = 10;
    
    // List yang menyimpan semua instance yang dibuat
    private List<GameObject> _pool;
    
    // Tidak perlu _poolContainer karena komponen ini sendiri adalah container
    // public GameObject Prefab => prefab; // Tambahkan ini jika dibutuhkan oleh script lain

    private void Awake()
    {
        _pool = new List<GameObject>();
        // Tidak perlu membuat pool container baru, karena script ini terpasang pada container
        CreatePooler();
    }

    private void CreatePooler()
    {
        for (int i = 0; i < poolSize; i++)
        {
            // Panggil Instantiate di sini, bukan di CreateInstance (agar lebih efisien)
            GameObject newInstance = Instantiate(prefab);
            
            // Set parent ke GameObject tempat script ini terpasang (Wadah Pool)
            newInstance.transform.SetParent(transform);
            newInstance.SetActive(false);
            
            // Tambahkan ke List
            _pool.Add(newInstance);
        }
    }

    // Fungsi untuk membuat instance baru JIKA pool habis (Extending Pool)
    private GameObject CreateNewInstance()
    {
        GameObject newInstance = Instantiate(prefab);
        newInstance.transform.SetParent(transform); // Set parent ke wadah ini
        newInstance.SetActive(false);
        _pool.Add(newInstance);
        
        Debug.LogWarning($"Pool '{gameObject.name}' melebihi batas {poolSize}. Membuat instance baru.");
        return newInstance;
    }

    public GameObject GetInstanceFromPool()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (!_pool[i].activeInHierarchy)
            {
                return _pool[i];
            }
        }
        
        // Jika pool habis, buat instance baru
        return CreateNewInstance();
    }

    // FUNGSI STATIS UNTUK MENGEMBALIKAN INSTANCE
    // Ini BUKAN fungsi pooler yang ideal, tapi ini sesuai dengan penggunaan Anda
    public static void ReturnToPool(GameObject instance)
    {
        // Cukup menonaktifkan, karena Spawner tidak menyimpan referensi ke List pool ini
        instance.SetActive(false);
    }

    public static IEnumerator ReturnToPoolWithDelay(GameObject instance, float delay)
    {
        yield return new WaitForSeconds(delay);
        instance.SetActive(false);
    }
}