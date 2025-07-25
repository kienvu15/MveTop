using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 5f;

    [Header("Damage")]
    public int damage = 1; // Số

    [SerializeField] private float currentHealth; // <-- hiện trong Inspector

    public System.Action OnDeath;
    public System.Action<float> OnDamaged;
    public System.Action<float> OnHealed;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnDamaged?.Invoke(damage);

        if (currentHealth <= 0f)
        {
            Die();
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

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        OnDeath?.Invoke();
        Destroy(gameObject); 
    }
}
