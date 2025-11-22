using UnityEngine;
using UnityEngine.UI;

public class GlobalUISound : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Masukkan file suara klik standar (mp3/wav) di sini")]
    public AudioClip standardClickSound;

    void Start()
    {
        Button[] allButtons = GetComponentsInChildren<Button>(true);

        foreach (Button btn in allButtons)
        {
            // Skip tombol yang sudah punya script ToggleButton (biar gak double suara)
            if (btn.GetComponent<ToggleButtonController>() != null)
            {
                continue; 
            }

            // Tambahkan suara ke tombol biasa
            btn.onClick.AddListener(() => PlaySound());
        }
    }

    void PlaySound()
    {
        // Panggil fungsi PlaySFX di SoundManager
        if (SoundManager.Instance != null && standardClickSound != null)
        {
            SoundManager.Instance.PlaySFX(standardClickSound);
        }
    }
}