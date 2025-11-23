using UnityEngine;
using TMPro; // Wajib ada jika pakai TextMeshPro
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Untuk pindah scene setelah selesai

public class DialogueManager : MonoBehaviour
{
    [Header("Komponen UI")]
    public TextMeshProUGUI textComponent; // Drag objek Text ke sini
    public GameObject bubbleObject;     
    public float typingSpeed = 0.05f;   // Drag objek Bubble ke sini (opsional)

    [Header("Isi Percakapan")]
    [TextArea(3, 10)] // Membuat kotak input di Inspector jadi lebar
    public string[] sentences; // Tulis dialog-dialogmu di sini di Inspector

    [Header("Setting")]
    public string nextSceneName; // Nama scene selanjutnya setelah dialog habis

    private int index = 0; // Untuk melacak kita ada di kalimat ke berapa

    void Start()
    {
        // Memulai dialog pertama saat game jalan
        textComponent.text = "";
        StartDialogue();
    }

    void StartDialogue()
    {
        index = 0;
        UpdateText();
    }

    // Fungsi ini dipanggil saat tombol Next ditekan
    public void NextSentence()
    {
        // Cek apakah masih ada kalimat selanjutnya?
        if (index < sentences.Length - 1)
        {
            index++;
            UpdateText();
        }
        else
        {
            // Jika kalimat sudah habis
            EndDialogue();
        }
    }

    // Tambahkan variabel kecepatan ketik


    void UpdateText()
    {
        // Hentikan ketikan sebelumnya jika user menekan tombol cepat-cepat
        StopAllCoroutines(); 
        StartCoroutine(TypeSentence(sentences[index]));
    }

    // Fungsi khusus untuk mengetik satu per satu
    System.Collections.IEnumerator TypeSentence(string sentence)
    {
        textComponent.text = ""; // Kosongkan dulu
        foreach (char letter in sentence.ToCharArray())
        {
            textComponent.text += letter; // Tambah 1 huruf
            yield return new WaitForSeconds(typingSpeed); // Tunggu sebentar
        }
    }

    void EndDialogue()
    {
        Debug.Log("Cutscene Selesai!");
        // Pindah ke scene gameplay atau matikan bubble
        // SceneManager.LoadScene(nextSceneName); 
        
        // Atau sembunyikan bubble chat:
        bubbleObject.SetActive(false);
    }
}