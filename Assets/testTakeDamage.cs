using UnityEngine;

public class testTakeDamage : MonoBehaviour
{
    public PlayerStats playerStats;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player entered the trigger area, applying damage.");
            playerStats.TakeDamage(1, transform.position);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player is still in the trigger area, applying damage.");
            playerStats.TakeDamage(1, transform.position);
        }
    }
}
