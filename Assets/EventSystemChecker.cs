using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemChecker : MonoBehaviour
{
    void Awake()
    {
        // Mencari semua objek yang memiliki komponen EventSystem
        EventSystem[] systems = FindObjectsOfType<EventSystem>();

        if (systems.Length > 1)
        {
            // Jika ada lebih dari 1, hancurkan objek ini (yang baru dibuat)
            // agar yang lama (persisten) tetap hidup.
            Destroy(gameObject);
        }
        else
        {
            // Jika ingin EventSystem ini bertahan antar scene:
            DontDestroyOnLoad(gameObject);
        }
    }
}