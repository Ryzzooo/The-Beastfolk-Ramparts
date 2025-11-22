using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dmgText;
    public TextMeshProUGUI DmgText => dmgText;
    // Fungsi ini akan kita panggil dari animasi
    public void SetDamage(float damageAmount)
    {
        // Mengubah angka float menjadi string teks
        dmgText.text = damageAmount.ToString();
    }
    public void OnAnimationFinished()
    {
        gameObject.SetActive(false);
    }
}
