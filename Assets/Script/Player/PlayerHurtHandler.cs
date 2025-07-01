using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHurtHandler : MonoBehaviour
{
    public PlayerStats stats;
    public float invincibleTime = 2f;
    public float blinkInterval = 0.2f;
    public float knockbackForce = 5f;

    private bool isInvincible = false;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ReceiveDamage(int damage, Vector2 attackerPosition)
    {
        if (isInvincible == true) return;

        stats.TakeDamage(damage);

        StartCoroutine(InvincibleCoroutine());
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;

        float elapsed = 0f;
        bool visible = true;

        while (elapsed < invincibleTime)
        {
            // Blink
            visible = !visible;
            if (spriteRenderer != null)
                spriteRenderer.enabled = visible;

            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        isInvincible = false;
    }
}
