using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 5f;

    [Header("Damage")]
    public int damage = 1; // Số

    [SerializeField] private float currentHealth; // <-- hiện trong Inspector
    private EnemyDeath enemyDeath;

    public System.Action OnDeath;
    public System.Action<float> OnDamaged;
    public System.Action<float> OnHealed;

    private void Awake()
    {
        currentHealth = maxHealth;
        enemyDeath = GetComponent<EnemyDeath>();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnDamaged?.Invoke(damage);

        if (currentHealth <= 0f)
        {
            enemyDeath.Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealed?.Invoke(amount);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }


}
