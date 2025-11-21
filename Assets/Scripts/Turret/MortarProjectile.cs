using UnityEngine;

// Kita warisi AOEProjectile agar tetap punya fitur ledakan area
public class MortarProjectile : AOEProjectile
{
    [Header("Mortar Specific")]
    [Tooltip("Tinggi lengkungan parabola")]
    [SerializeField] private float arcHeight = 2.0f;
    
    [Tooltip("Kecepatan putaran peluru agar moncongnya menghadap arah gerak")]
    [SerializeField] private bool rotateToDirection = true;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private float _progress; // Nilai 0 sampai 1 (0 = awal, 1 = sampai)
    private float _flightDuration; // Berapa lama peluru di udara
    private bool _isLaunched = false;

    private void OnEnable()
    {
        // Pastikan saat baru lahir, dia belum boleh terbang
        _isLaunched = false;
        _progress = 0f;
    }

    // Kita override fungsi SetTarget untuk mengunci posisi awal & tujuan
    public override void SetTarget(Enemy enemy)
    {
        base.SetTarget(enemy); // Panggil logika dasar (simpan _target)

        _startPos = transform.position;
        
        // PENTING: Mortar biasanya menarget POSISI TANAH saat ditembakkan.
        // Jadi meskipun musuh lari, peluru tetap jatuh di titik itu.
        // Kalau mau homing (ngejar), logika ini harus diubah sedikit.
        _targetPos = enemy.transform.position;

        // Hitung jarak untuk menentukan durasi terbang
        // (Semakin jauh, semakin lama sampainya, tapi speed tetap konsisten)
        float distance = Vector3.Distance(_startPos, _targetPos);
        _flightDuration = distance / Mathf.Max(moveSpeed, 0.1f);
        
        _progress = 0f;
        _isLaunched = true;
    }

    // Kita override Update karena gerakannya beda total dengan Projectile biasa
    protected override void Update()
    {
        if (!_isLaunched) return;
        // Cek jika target null (misal musuh mati duluan sebelum peluru sampai)
        // Kita tetap lanjutkan peluru jatuh ke tanah (targetPos)
        
        // Tambahkan progress berdasarkan waktu
        _progress += Time.deltaTime / _flightDuration;

        if (_progress >= 1.0f)
        {
            // Sudah sampai
            HitTarget();
            return;
        }

        // --- RUMUS PARABOLA (Bezier Curve Sederhana) ---
        
        // 1. Hitung posisi lurus (Linear) dari A ke B
        Vector3 currentPos = Vector3.Lerp(_startPos, _targetPos, _progress);

        // 2. Tambahkan ketinggian (Y) berdasarkan progress
        // Rumus: 4 * tinggi * x * (1 - x) --> Ini membentuk kurva naik lalu turun
        // Saat progress 0 -> tinggi 0
        // Saat progress 0.5 -> tinggi maks
        // Saat progress 1 -> tinggi 0
        float height = arcHeight * 4 * _progress * (1 - _progress);
        
        // Terapkan posisi baru
        currentPos.y += height;
        currentPos.z = 0;
        transform.position = currentPos;

        // --- ROTASI PELURU (Agar moncong menghadap arah terbang) ---
        if (rotateToDirection)
        {
            // Hitung arah dari posisi sebelumnya (atau prediksi sedikit ke depan)
            float nextProgress = _progress + 0.05f;
            Vector3 nextPosLinear = Vector3.Lerp(_startPos, _targetPos, nextProgress);
            float nextHeight = arcHeight * 4 * nextProgress * (1 - nextProgress);
            nextPosLinear.y += nextHeight;

            Vector3 direction = nextPosLinear - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            
            // Sesuaikan offset -90 atau 0 tergantung sprite kamu
           transform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }

    // Override HitTarget agar tetap meledak di posisi terakhir
    // meskipun musuh aslinya sudah pindah/mati
    protected override void HitTarget()
    {
        _isLaunched = false;
        // Panggil logika ledakan dari AOEProjectile
        // Tapi kita perlu sedikit trik: AOEProjectile mungkin butuh _target yang valid.
        // Karena ini mortar, kita ledakkan saja di posisi peluru sekarang.
        
        // Copas logika overlap dari AOEProjectile, atau pastikan base.HitTarget aman.
        // Cara paling aman: Panggil base.HitTarget() tapi pastikan dia tidak error kalau _target null.
        
        // KITA TULIS ULANG SEDIKIT LOGIKA LEDAKANNYA AGAR AMAN:
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, 3f); // Hardcode radius atau bikin protected di AOE
        
        // ... (Logika damage sama seperti AOEProjectile) ...
        foreach (Collider2D enemyCollider in hitEnemies)
        {
            if (enemyCollider.CompareTag("Enemy"))
            {
                EnemyHealth eh = enemyCollider.GetComponent<EnemyHealth>();
                if (eh != null) eh.DealDamage(damage);
            }
        }
        
        // Efek visual ledakan disini
        
        gameObject.SetActive(false);
    }
}