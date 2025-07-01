using System.Collections;
using UnityEngine;

public class EnemyShot : MonoBehaviour
{
    public GameObject bulletPrefab;

    public float bulletSpeed = 10f;
    public int bulletCount = 1;
    public float fireInterval = 0.2f;
    public float lockTime = 1f;

    public float cooldownDuration = 2f; // ⏱️ Cooldown sau mỗi loạt bắn
    public float cooldownTimer = 0f;

    private EnemyAttackVision attackVision;
    private EnemyVision vision;

    public float visionTimer = 0f;
    public bool isShooting = false;
    public bool isLocked = false;
    public bool isInCooldown = false;

    private Rigidbody2D rb;
    public event System.Action OnShotFinished;
    private void Start()
    {
        attackVision = GetComponent<EnemyAttackVision>();
        vision = GetComponent<EnemyVision>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {

    }

    public void ConditionShot()
    {
        // ⏳ Đang cooldown thì đếm thời gian
        if (isInCooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer >= cooldownDuration)
            {
                isInCooldown = false;
                cooldownTimer = 0f;
            }
            return;
        }

        if (!isShooting)
        {
            HandleLockAndShoot();
        }
    }

    public void HandleLockAndShoot()
    {
        if (vision.CanSeePlayer)
        {
            visionTimer += Time.deltaTime;

            if (visionTimer >= lockTime && !isLocked)
            {
                
                if (vision.targetDetected != null)
                {
                    attackVision.MoveAttackPointToPlayer();
                    isLocked = true;
                    StartCoroutine(ShootCoroutine());
                }
            }
        }
        else
        {
            visionTimer = 0f;
            isLocked = false;
        }
    }

    private IEnumerator ShootCoroutine()
    {
        isShooting = true;
        rb.linearVelocity = Vector2.zero;
        for (int i = 0; i < bulletCount; i++)
        {
            ShootBullet();
            if (bulletCount > 1 && i < bulletCount - 1)
                yield return new WaitForSeconds(fireInterval);
        }

        yield return new WaitForSeconds(0.1f);

        // 🔁 Sau khi bắn xong: bắt đầu cooldown
        OnShotFinished?.Invoke(); Debug.Log("Shot stage finished and event invoked.");
        isShooting = false;
        isLocked = false;
        visionTimer = 0f;

        isInCooldown = true;
        cooldownTimer = 0f;
    }

    private void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, attackVision.attackPoint.position, attackVision.attackPoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = attackVision.attackPoint.right * bulletSpeed;
        }
    }
}
