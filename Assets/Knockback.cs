using UnityEngine;

public class Knockback : MonoBehaviour
{
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;

    private Rigidbody2D rb;
    private float knockbackTimer;

    public bool IsMovementLocked => knockbackTimer > 0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector3 sourcePosition)
    {
        Vector2 knockDirection = (transform.position - sourcePosition).normalized;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockDirection * knockbackForce, ForceMode2D.Impulse);

        knockbackTimer = knockbackDuration;
        // Gọi tới AI để báo "bị đánh"
        SendMessage("OnHitInterrupt", SendMessageOptions.DontRequireReceiver);
    }

    void Update()
    {
        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
        }
    }
}
