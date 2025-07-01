using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int maxArmor = 50;

    public int currentHealth;
    public int currentArmor;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
    }

    public void TakeDamage(int damage)
    {
        if (currentArmor > 0)
        {
            int remainingDamage = damage - currentArmor;
            currentArmor -= damage;
            if (currentArmor < 0)
                currentArmor = 0;

            if (remainingDamage > 0)
            {
                currentHealth -= remainingDamage;
            }
        }
        else
        {
            currentHealth -= damage;
        }

        Debug.Log($"{gameObject.name} bị chém! Máu còn: {currentHealth}, Giáp còn: {currentArmor}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " đã chết!");
        Destroy(gameObject);
        
    }

    
}
