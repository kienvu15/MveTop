using System.Collections;
using UnityEngine;

public class BulletSwapm : MonoBehaviour
{
    [Header("Control Setting")]
    [SerializeField] public int bulletCount = 15;
    [SerializeField] public float bulletSpeed = 5f;
    [SerializeField] public float shootCooldown = 5f;


    [Header("Round")]
    [SerializeField] public GameObject bulletPrefab;

    [Header("Cone")]
    [SerializeField] public GameObject bulletPrefab2;
    public float coneAngle = 60f;

    [Header("ShotGun")]
    [SerializeField] private float shootDelay = 0.08f; // Delay giữa các viên
    [SerializeField] private float spreadAngle = 10f; // Độ tán xạ ± góc

    public bool isShooting = false;
    private bool isOnCooldown = false;
    private EnemyAttackVision enemyAttackVision;
    public event System.Action OnShotBulletFinished;

    private void Awake()
    {
        enemyAttackVision = GetComponent<EnemyAttackVision>();
    }

    public void Update()
    {
        
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

            yield return new WaitForSeconds(shootDelay); // Delay nhỏ giữa các viên
        }
    }
}
