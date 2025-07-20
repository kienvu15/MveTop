using System.Collections;
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
    [SerializeField] public float recoilHitForce = 9f;

    [Header("ComboSetting")]
    public int comboStep = 0;
    public float dashOnHitSpeed = 2f;
    public float finalDashSpeed = 10f;
    public float attackdashDuration = 0.2f;

    [Header("Invincibility")]
    public float invincibilityDuration = 1f;   // 1 đến 2 giây
    private bool isInvincible = false;
    private float invincibilityTimer = 0f;


    [Space(20)]
    public PlayerUI playerUI;
    public SpriteRenderer spriteRenderer;  // Gán Sprite Renderer vào đây
    public PlayerRecoil playerRecoil;

    void Start()
    {

        playerUI = FindFirstObjectByType<PlayerUI>();
        playerRecoil = GetComponent<PlayerRecoil>();

        currentHealth = maxHealth;
        currentArmor = maxArmor;

        UpdateUI();

    }

    private void Update()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0f)
            {
                isInvincible = false;
            }
        }
    }

    public void UpdateUI()
    {
        if (playerUI != null)
        {
            playerUI.SetHealth(currentHealth, maxHealth);
            playerUI.SetAmmo(currentArmor, maxArmor);
        }
    }

    private Coroutine flashCoroutine;

    private IEnumerator FlashWhileInvincible()
    {
        float flashInterval = 0.1f;  // Thời gian giữa mỗi lần nhấp nháy

        while (isInvincible)
        {
            spriteRenderer.enabled = false;
            yield return new WaitForSeconds(flashInterval);
            spriteRenderer.enabled = true;
            yield return new WaitForSeconds(flashInterval);
        }

        spriteRenderer.enabled = true;  // Đảm bảo bật sáng trở lại khi hết bất tử
    }

    public void TakeDamage(int damage, Vector2 attackPointPosition)
    {
        if (isInvincible)
        {
            Debug.Log("Player is invincible! No damage taken.");
            return;
        }

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

        if (playerRecoil != null)
        {
            playerRecoil.ApplyHitedRecoil(attackPointPosition);
        }

        // Bắt đầu bất tử
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashWhileInvincible());

        
        
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
