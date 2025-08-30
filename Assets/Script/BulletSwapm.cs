using System.Collections;
using UnityEngine;

public class BulletSwapm : MonoBehaviour
{
    [Header("Control Setting")]
    [SerializeField] public int bulletCount = 15;
    [SerializeField] public float bulletSpeed = 5f;
    [SerializeField] public float shootCooldown = 5f;
    public Transform playerTransform;

    [Header("Round")]
    [SerializeField] public GameObject bulletPrefab;

    [Header("Cone")]
    [SerializeField] public GameObject bulletPrefab2;
    public float coneAngle = 60f;

    [Header("ShotGun")]
    [SerializeField] private float shootDelay = 0.08f; // Delay giữa các viên
    [SerializeField] private float spreadAngle = 10f; // Độ tán xạ ± góc

    [Header("Missile")]
    [SerializeField] private GameObject missilePrefab;
    public EnemyStats enemyStats;

    public bool isShooting = false;
    public bool isOnCooldown = false;
    private EnemyAttackVision enemyAttackVision;
    public event System.Action OnShotBulletFinished;

    public float time;
    public float duration = 2f;

    private void Awake()
    {
        enemyAttackVision = GetComponent<EnemyAttackVision>();
        enemyStats = GetComponent<EnemyStats>();
    }

    public void Start()
    {
        playerTransform = FindFirstObjectByType<PlayerStats>().transform;
        if (playerTransform == null)
        {
            Debug.LogError("PlayerStats not found in the scene.");
        }
    }
    public void Update()
    {
        //time += Time.deltaTime;
        //if (time >= duration)
        //{
        //    ShotFourDirectionsCondition();
        //}
    }

    public void ShotCondition()
    {
        if (!isOnCooldown)
        {
            StartCoroutine(ShotRoutine());
        }
    }

    private IEnumerator ShotRoutine()
    {
        isShooting = true;
        isOnCooldown = true;

        ShootCircle();
        OnShotBulletFinished?.Invoke();

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
        isOnCooldown = false;
    }
    public void ShootCircle()
    {
        for (int i = 0; i < bulletCount; i++)
        {
            float angle = i * 360f / bulletCount; // chia đều 360 độ
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.enemyStats = this.enemyStats;
            }
            // Xoay bullet sao cho trục X hướng về dir
            float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angleDeg);

            // Di chuyển
            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * bulletSpeed;

        }
    }

    

    private IEnumerator ShotConeRoutine()
    {
        isShooting = true;
        isOnCooldown = true;

        ShootCone(enemyAttackVision.attackPoint.right);

        OnShotBulletFinished?.Invoke();

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
        isOnCooldown = false;
    }

    public void ShootCone(Vector2 direction)
    {
        direction.Normalize();
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float startAngle = baseAngle - coneAngle / 2f;
        float angleStep = coneAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            float currentAngle = startAngle + angleStep * i;
            Vector2 dir = new Vector2(Mathf.Cos(currentAngle * Mathf.Deg2Rad), Mathf.Sin(currentAngle * Mathf.Deg2Rad));

            GameObject bullet = Instantiate(bulletPrefab2, transform.position, Quaternion.identity);
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.enemyStats = this.enemyStats;
            }
            bullet.transform.rotation = Quaternion.Euler(0, 0, currentAngle);

            bullet.GetComponent<Rigidbody2D>().linearVelocity = dir.normalized * bulletSpeed;
        }
    }

    /// <summary>
    /// Shotgun condition to check if the shotgun can be fired.
    /// </summary>
    
    public void ShotGunCondition()
    {
        if (!isOnCooldown)
        {
            StartCoroutine(ShotGunRoutine(enemyAttackVision.attackPoint.right));
        }
    }

    private IEnumerator ShotGunRoutine(Vector2 direction)
    {
        isShooting = true;
        isOnCooldown = true;

        StartCoroutine(ShootShotgunBurst(direction));
        OnShotBulletFinished?.Invoke();

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
        isOnCooldown = false;
    }

    private IEnumerator ShootShotgunBurst(Vector2 direction)
    {
        direction.Normalize();
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        for (int i = 0; i < bulletCount; i++)
        {
            float randomOffset = Random.Range(-spreadAngle, spreadAngle);
            float shotAngle = baseAngle + randomOffset;

            Vector2 finalDir = new Vector2(Mathf.Cos(shotAngle * Mathf.Deg2Rad), Mathf.Sin(shotAngle * Mathf.Deg2Rad));

            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.Euler(0, 0, shotAngle));
            bullet.GetComponent<Rigidbody2D>().linearVelocity = finalDir.normalized * bulletSpeed;
            Bullet bulletComponent = bullet.GetComponent<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.enemyStats = this.enemyStats;
            }
            yield return new WaitForSeconds(shootDelay); // Delay nhỏ giữa các viên
        }
    }

    /// <summary>
    /// Missile condition to check if the missile can be fired.
    /// </summary>

    public void ShotMissileCondition()
    {
        if (!isOnCooldown)
        {
            StartCoroutine(ShotMissileRoutine(enemyAttackVision.attackPoint.right));
        }
    }

    private IEnumerator ShotMissileRoutine(Vector2 direction)
    {
        isShooting = true;
        isOnCooldown = true;

        ShootMissile();
        OnShotBulletFinished?.Invoke();

        yield return new WaitForSeconds(shootCooldown);
        isShooting = false;
        isOnCooldown = false;
    }

    public void ShootMissile()
    {
        GameObject bullet = Instantiate(missilePrefab, enemyAttackVision.attackPoint.position, enemyAttackVision.attackPoint.rotation);

        HomingBullet homing = bullet.GetComponent<HomingBullet>();
        if (homing != null)
            homing.target = playerTransform;

        Missile missile = bullet.GetComponent<Missile>();
        if (missile != null)
        {
            missile.enemyStats = this.enemyStats;
        }
    }

    /// <summary>
    /// Four-direction shoot condition (Up, Down, Left, Right).
    /// </summary>
    public void ShotFourDirectionsCondition()
    {
        if (!isOnCooldown)
        {
            StartCoroutine(ShotFourDirectionsRoutine());
        }
    }

    private IEnumerator ShotFourDirectionsRoutine()
    {
        isShooting = true;
        isOnCooldown = true;

        ShootFourDirectionsHoming();
        OnShotBulletFinished?.Invoke();

        yield return new WaitForSeconds(2f); // cooldown 2s
        isShooting = false;
        isOnCooldown = false;
    }

    public void ShootFourDirectionsHoming()
    {
        Vector2[] directions = new Vector2[]
        {
        Vector2.right,
        Vector2.left,
        Vector2.up,
        Vector2.down
        };

        foreach (var dir in directions)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);

            float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            bullet.transform.rotation = Quaternion.Euler(0, 0, angleDeg);

            HomingBullet homing = bullet.GetComponent<HomingBullet>();
            if (homing != null)
            {
                homing.target = playerTransform;
                homing.currentDirection = dir.normalized; // 👈 ép hướng ban đầu
                homing.rb.linearVelocity = dir.normalized * homing.startSpeed;
            }

            Missile missile = bullet.GetComponent<Missile>();
            if (missile != null)
            {
                missile.enemyStats = this.enemyStats;
            }
        }
    }




}
