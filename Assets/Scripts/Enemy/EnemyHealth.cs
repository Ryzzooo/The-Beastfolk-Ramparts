using UnityEngine;
using UnityEngine.UI;
using System;

public class EnemyHealth : MonoBehaviour
{
    public static Action<Enemy> OnEnemyKilled;
    public static Action<Enemy> OnEnemyHit;

    [SerializeField] private GameObject healthBarPrefab;
    [SerializeField] private Transform barPosition;
    [SerializeField] private float initialHealth = 10f;
    [SerializeField] private float maxHealth = 10f;

    public float CurrentHealth { get; set; }

    private Image _healthBar;
    private Enemy _enemy;

    private void OnEnable()
    {
        // Cek ini agar health bar tidak dibuat ulang setiap kali
        if (_healthBar == null)
        {
            CreateHealthBar();
        }

        // Reset health setiap kali spawn
        CurrentHealth = initialHealth;
        _healthBar.fillAmount = 1f; // Langsung set ke penuh

        if (_enemy == null)
        {
            _enemy = GetComponent<Enemy>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            DealDamage(5f);
        }

        _healthBar.fillAmount = Mathf.Lerp(_healthBar.fillAmount,
            CurrentHealth / maxHealth, Time.deltaTime * 10f);
    }

    private void CreateHealthBar()
    {
        GameObject newBar = Instantiate(healthBarPrefab, barPosition.position, Quaternion.identity);
        newBar.transform.SetParent(transform);
        EnemyHealthContainer container = newBar.GetComponent<EnemyHealthContainer>();
        _healthBar = container.FillAmountImage;
    }

    public void DealDamage(float damageReceived)
    {
        CurrentHealth -= damageReceived;
        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Die();
        }
        else
        {
            OnEnemyHit?.Invoke(_enemy);
        }
    }

    public void ResetHealth()
    {
        CurrentHealth = maxHealth;
    }

    private void Die()
{
    OnEnemyKilled?.Invoke(_enemy);
    ResetHealth(); // Reset health untuk di pool
    ObjectPooler.ReturnToPool(gameObject);
}
}