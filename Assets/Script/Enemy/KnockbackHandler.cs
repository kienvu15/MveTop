using UnityEngine;
using System.Collections;

public class KnockbackHandler : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isKnockedBack = false;

    [SerializeField] private float knockbackForce = 3f;
    [SerializeField] private float knockbackDuration = 0.1f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 direction)
    {
        if (!isKnockedBack)
        {
            StartCoroutine(HandleKnockback(direction));
        }
    }

    private IEnumerator HandleKnockback(Vector2 direction)
    {
        isKnockedBack = true;

        float timer = 0f;
        while (timer < knockbackDuration)
        {
            rb.linearVelocity = direction.normalized * knockbackForce;
            timer += Time.deltaTime;
            yield return null;
        }

        isKnockedBack = false;
    }
}
