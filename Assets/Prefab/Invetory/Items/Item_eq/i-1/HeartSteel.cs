using UnityEngine;

public class HeartSteel : Item
{
    public PlayerStats playerStats;
    public int originalMaxHealth;
    private bool hasAppliedEffect = false;
    private void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        originalMaxHealth = playerStats.OriginalMaxHealth;
    }

    public override void ApplyEffect(GameObject player)
    {
        if (playerStats == null)
            playerStats = player.GetComponent<PlayerStats>();

        if (!hasAppliedEffect)
        {
            originalMaxHealth = playerStats.maxHealth;
            playerStats.maxHealth *= 2;
            playerStats.currentHealth = playerStats.maxHealth;
            playerStats.UpdateUI();
            hasAppliedEffect = true;

            Debug.Log($"[+] {Name}: maxHealth x2");
        }
    }


    public override void RemoveEffect(GameObject player)
    {
        playerStats.maxHealth = originalMaxHealth;
        playerStats.currentHealth = Mathf.Min(playerStats.currentHealth, originalMaxHealth);

        playerStats.UpdateUI();
        Debug.Log($"Messi");
    }

}
    

    //private bool isEffectApplied = false;

   

    //public void Appl2yEffect()
    //{
    //    if (isEffectApplied) return;

    //    playerStats.maxHealth *= 2;
    //    playerStats.currentHealth = playerStats.maxHealth;
    //    transform.localScale = originalScale * 2f;

    //    playerStats.UpdateUI();
    //    isEffectApplied = true;
    //}

    //public void Remov2eEffect()
    //{
    //    if (!isEffectApplied) return;

    //    playerStats.maxHealth = originalMaxHealth;
    //    playerStats.currentHealth = Mathf.Min(playerStats.currentHealth, originalMaxHealth);
    //    transform.localScale = originalScale;

    //    playerStats.UpdateUI();
    //    isEffectApplied = false;
    //}

