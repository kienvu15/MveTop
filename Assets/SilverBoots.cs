using UnityEngine;

public class SilverBoots : Item
{
    public PlayerStats playerStats;
    public int originalMoveSpeed;
    private bool hasAppliedEffect = false;
    private void Awake()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
        originalMoveSpeed = 7;
    }

    public override void ApplyEffect(GameObject player)
    {
        if (playerStats == null)
            playerStats = player.GetComponent<PlayerStats>();

        if (!hasAppliedEffect)
        {
            playerStats.moveSpeed = 8.5f;

            hasAppliedEffect = true;

            Debug.Log($"[+] {Name}: moveSpeed x2");
        }
    }


    public override void RemoveEffect(GameObject player)
    {
        playerStats.moveSpeed = originalMoveSpeed;

        playerStats.UpdateUI();
        Debug.Log($"[-] {Name}: moveSpeed /2");
    }

}