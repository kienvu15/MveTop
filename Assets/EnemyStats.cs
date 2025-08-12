using System.Collections;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 5f;

    [Header("Damage")]
    public int damage = 1; // Số

    [Header("Recoil Settings")]
    public float recoilHitForce = 0.8f; // Lực đẩy khi bị tấn công
    public float recoilForceOriginal = 0.8f;
    public SpriteRenderer spriteRenderer;  // Gán Sprite Renderer vào đây

    [SerializeField] private float currentHealth; // <-- hiện trong Inspector
    private EnemyDeath enemyDeath;

    public System.Action OnDeath;
    public System.Action<float> OnDamaged;
    public System.Action<float> OnHealed;

    //Refer
    private Rigidbody2D rb;
    public PlayerStats playerStats;
    public EnemyStateController enemyStateController;
    private void Awake()
    {
        currentHealth = maxHealth;
        enemyDeath = GetComponent<EnemyDeath>();
        rb = GetComponent<Rigidbody2D>();
        enemyStateController = GetComponent<EnemyStateController>();
        playerStats = GameObject.FindGameObjectWithTag("Player")?.GetComponent<PlayerStats>();
    }

    public void TakeDamage(float damage, Vector2 attackPointPosition)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnDamaged?.Invoke(damage);

        ApplyHitedRecoil(attackPointPosition);

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
            enemyDeath.Die();
        }
    }

    public void TakeDamageNoRecoil(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnDamaged?.Invoke(damage);
        enemyStateController.isRecoiling = true;

        ApplyNoHitedRecoil();

        if (currentHealth <= 0f)
        {
            OnDeath?.Invoke();
            enemyDeath.Die();
        }

    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealed?.Invoke(amount);
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    private Coroutine flashCoroutine;
    private IEnumerator FlashWhileInvincible()
    {
        float flashInterval = 0.1f;  // Thời gian giữa mỗi lần nhấp nháy

        while (enemyStateController.isRecoiling == true)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.enabled = true;  // Đảm bảo bật sáng trở lại khi hết bất tử
    }

    public void ApplyHitedRecoil(Vector2 attackPointPosition)
    {
        StopAllCoroutines(); // Dừng recoil cũ nếu có
        StartCoroutine(RecoilTweenRoutine(attackPointPosition));
    }

    private IEnumerator RecoilTweenRoutine(Vector2 attackPointPosition)
    {
        enemyStateController.isRecoiling = true;

        Vector2 startPos = transform.position;
        Vector2 dir = ((Vector2)transform.position - attackPointPosition).normalized;
        Vector2 targetPos = startPos + dir * recoilHitForce; // khoảng cách đẩy

        float duration = 0.2f;
        float timer = 0f;

        while (timer < duration)
        {
            float t = timer / duration;
            rb.MovePosition(Vector2.Lerp(startPos, targetPos, t));
            timer += Time.deltaTime;
            yield return null;
        }

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashWhileInvincible());

        yield return new WaitForSeconds(0.7f); // tạm dừng sau khi knockback
        enemyStateController.isRecoiling = false;
    }

    public void ApplyNoHitedRecoil()
    {
        StopAllCoroutines(); // Dừng recoil cũ nếu có
        StartCoroutine(NoRecoilTweenRoutine());
    }

    private IEnumerator NoRecoilTweenRoutine()
    {
        enemyStateController.isRecoiling = true;

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashWhileInvincible());

        yield return new WaitForSeconds(1.2f); // tạm dừng sau khi knockback
        enemyStateController.isRecoiling = false;
    }
}
