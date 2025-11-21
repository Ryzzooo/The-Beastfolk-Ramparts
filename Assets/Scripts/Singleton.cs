using UnityEngine;

/// <summary>
/// Singleton generik yang akan membuat instance-nya sendiri
/// dan bisa diakses dari mana saja via .Instance
/// </summary>
/// <typeparam name="T">Komponen yang akan menjadi Singleton</typeparam>
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                // Coba cari dulu di scene
                _instance = FindFirstObjectByType<T>();
                
                if (_instance == null)
                {
                    // Jika tidak ada, buat GameObject baru
                    GameObject obj = new GameObject();
                    obj.name = typeof(T).Name;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            // Opsional: jangan hancurkan saat ganti scene
            // DontDestroyOnLoad(gameObject); 
        }
        else if (_instance != this)
        {
            // Jika sudah ada instance lain, hancurkan yang ini
            Destroy(gameObject);
        }
    }
}