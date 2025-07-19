using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 6;
    public int currentHealth;

    [Header("Armor")]
    public int maxArmor = 4;
    public int currentArmor;

    [Header("MoveSpeed")]
    public float moveSpeed = 5f;

    [Header("Dash")]
    [SerializeField] public float dashSpeed = 15f;
    [SerializeField] public float dashDuration = 0.2f;
    [SerializeField] public float dashCooldown = 0.5f;

    [Header("Attack Settings")]
    public float attackRange = 1f;
    public float attackRadius = 0.5f;
    public int damage = 2;

    [Header("Recoil")]
    [SerializeField] public float recoilForce = 5f;

    [Header("ComboSetting")]
    public int comboStep = 0;
    public float dashOnHitSpeed = 2f;
    public float finalDashSpeed = 10f;
    public float attackdashDuration = 0.2f;

    [Space(20)]
    public Slider healthBar;
    public Slider armorBar;

    void Start()
    {
        currentHealth = maxHealth;
        currentArmor = maxArmor;
    }

    public void UpdateUI()
    {
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }

        if (armorBar != null)
        {
            armorBar.maxValue = maxArmor;
            armorBar.value = currentArmor;
        }
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
        UpdateUI();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateUI();
    }

    public void RestoreArmor(int amount)
    {
        currentArmor = Mathf.Min(currentArmor + amount, maxArmor);
        UpdateUI();
    }

}
