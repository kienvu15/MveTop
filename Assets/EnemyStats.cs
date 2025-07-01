using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 6;
    public int currentHealth;

    [Header("Armor")]
    public int maxArmor = 4;
    public int currentArmor;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
    }

    public void TakeDamage(int damage)
    {
        int remainingDamage = damage;

        if (currentArmor > 0)
        {
            int absorbed = Mathf.Min(currentArmor, remainingDamage);
            currentArmor -= absorbed;
            remainingDamage -= absorbed;
        }

        if (remainingDamage > 0)
        {
            currentHealth = Mathf.Max(currentHealth - remainingDamage, 0);
        }

        Debug.Log($"Armor: {currentArmor}, Health: {currentHealth}");
        
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);       
    }

    public void RestoreArmor(int amount)
    {
        currentArmor = Mathf.Min(currentArmor + amount, maxArmor);
    }
}
