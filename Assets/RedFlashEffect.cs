using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RedFlashEffect : MonoBehaviour
{
    private Image _image;
    
    [Header("Settings")]
    // Nilai Max Alpha di Inspector sekarang 1f, yang bagus untuk pengujian visual.
    [SerializeField] private float flashDuration = 0.1f; 
    [SerializeField] private float maxAlpha = 1f;       
    
    private void Awake()
    {
        _image = GetComponent<Image>();
        // Pastikan Image tidak terlihat di awal
        Color c = _image.color;
        // PENTING: Jika warna Image Anda di Inspector BUKAN MERAH, Anda harus set di sini:
        // c.r = 1f; c.g = 0f; c.b = 0f; 
        c.a = 0f;
        _image.color = c;

        Debug.Log("Alpha Awal Image: " + _image.color.a); 
    }

    public void Flash()
    {
        Debug.Log("RedFlashEffect: Flash dipanggil!"); 

        // Hentikan Coroutine sebelumnya dan mulai yang baru
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Fade In (Cepat)
        float timer = 0f;
        while (timer < flashDuration)
        {
            timer += Time.deltaTime;
            // Lerp dari 0f (transparan) ke maxAlpha (puncak merah)
            float alpha = Mathf.Lerp(0f, maxAlpha, timer / flashDuration);
            SetAlpha(alpha);
            Debug.Log("Alpha Fade In: " + alpha); 
            yield return null;
        }
        
        // Pastikan di alpha maksimum sebelum fade out
        SetAlpha(maxAlpha);

        // 2. Fade Out (Lebih lambat)
        timer = 0f;
        while (timer < flashDuration * 2f) 
        {
            timer += Time.deltaTime;
            // Lerp dari maxAlpha ke 0f (transparan)
            float alpha = Mathf.Lerp(maxAlpha, 0f, timer / (flashDuration * 2f));
            SetAlpha(alpha);
            yield return null;
        }

        // Pastikan Image benar-benar transparan di akhir
        SetAlpha(0f);
    }

    // FUNGSI INI SUDAH DIPERBAIKI
    private void SetAlpha(float alpha)
    {
        Color c = _image.color;
        
        // Cukup panggil Clamp01 untuk memastikan alpha valid dan menetapkannya
        c.a = Mathf.Clamp01(alpha); 
        
        _image.color = c;
    }
}