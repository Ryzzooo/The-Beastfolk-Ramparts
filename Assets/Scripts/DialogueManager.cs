using UnityEngine;
using TMPro; // Wajib ada jika pakai TextMeshPro
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Untuk pindah scene setelah selesai

[System.Serializable]
public class DialogueLine
{
    [Header("Isi Percakapan per Baris")]
    [TextArea(3, 10)]
    public string sentence;      // Teks yang diucapkan
    public Sprite characterSprite; // Gambar karakter yang ngomong saat itu
}

public class DialogueManager : MonoBehaviour
{
    [Header("Komponen UI")]
    public TextMeshProUGUI textComponent; // Drag objek Text ke sini
    public GameObject bubbleObject;   
    public Image characterImageDisplay;  

    [Header("Data Percakapan")]
    // Kita tidak pakai string[] lagi, tapi pakai DialogueLine[]
    public DialogueLine[] dialogueLines;
    private int index = 0;
    public float typingSpeed = 0.05f;

    [Header("Setting")]
    public string nextSceneName; // Nama scene selanjutnya setelah dialog habis
 // Untuk melacak kita ada di kalimat ke berapa

    void Start()
    {
        // Memulai dialog pertama saat game jalan
        textComponent.text = "";
        StartDialogue();
    }

    void StartDialogue()
    {
        index = 0;
        bubbleObject.SetActive(true); // Pastikan bubble muncul di awal
        UpdateContent();
    }

    // Fungsi ini dipanggil saat tombol Next ditekan
    public void NextSentence()
    {
        if (textComponent.text == dialogueLines[index].sentence)
        {
            // Jika sudah selesai mengetik, lanjut ke kalimat berikutnya
            if (index < dialogueLines.Length - 1)
            {
                index++;
                UpdateContent();
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            // Jika sedang mengetik, langsung tampilkan semua teks (skip ketikan)
            StopAllCoroutines();
            textComponent.text = dialogueLines[index].sentence;
        }
    }

    // Tambahkan variabel kecepatan ketik


    void UpdateContent()
    {
        StopAllCoroutines();
        
        // Ambil data baris saat ini
        DialogueLine currentLine = dialogueLines[index];

        // 1. Ganti Gambar Karakter sesuai data di baris ini
        if (currentLine.characterSprite != null)
        {
            characterImageDisplay.sprite = currentLine.characterSprite;
            characterImageDisplay.gameObject.SetActive(true); // Pastikan gambarnya aktif
        }
        else
        {
            // Opsional: Sembunyikan gambar jika slot sprite dikosongkan (misal narator/suara hati)
            characterImageDisplay.gameObject.SetActive(false);
        }

        // 2. Mulai efek mengetik untuk teksnya
        StartCoroutine(TypeSentence(currentLine.sentence));
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
        bubbleObject.SetActive(false);
        characterImageDisplay.gameObject.SetActive(false); // Sembunyikan karakter di akhir
        SceneManager.LoadScene(nextSceneName); 
    }
}