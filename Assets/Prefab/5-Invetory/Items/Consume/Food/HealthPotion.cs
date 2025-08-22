using UnityEngine;

public class HealthPotion : MonoBehaviour
{
    public int healAmount = 2;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.Heal(healAmount);
                Destroy(gameObject); // Xoá item sau khi ăn
            }
        }
    }
}
