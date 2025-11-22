using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dmgText;
    public TextMeshProUGUI DmgText => dmgText;
    // Fungsi ini akan kita panggil dari animasi
    public void SetText(string text)
    {
        dmgText.text = text;
    }

    // 2. Fungsi untuk mengubah warna (misal: jadi Kuning)
    public void SetColor(Color color)
    {
        dmgText.color = color;
    }
    public void OnAnimationFinished()
    {
        gameObject.SetActive(false);
    }
}
