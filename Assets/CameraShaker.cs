using UnityEngine;
using System.Collections;

public class CameraShaker : MonoBehaviour
{
    // Instance tunggal untuk akses mudah (Singleton)
    public static CameraShaker Instance; 
    
    private Vector3 _originalPosition;
    
    private void Awake()
    {
        // Implementasi dasar Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        _originalPosition = transform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        StopAllCoroutines(); // Hentikan goyangan sebelumnya jika sedang berjalan
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // Ambil posisi acak kecil dalam lingkaran (X dan Y)
            float x = Random.Range(-2f, 2f) * magnitude;
            float y = Random.Range(-2f, 2f) * magnitude;

            // Terapkan pergeseran ke posisi kamera
            transform.localPosition = _originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            // Kurangi magnitude seiring waktu agar goyangan mereda (opsional, tapi disarankan)
            magnitude = Mathf.Lerp(magnitude, 0f, elapsed / duration); 

            yield return null;
        }

        // Kembalikan ke posisi semula
        transform.localPosition = _originalPosition;
    }
}