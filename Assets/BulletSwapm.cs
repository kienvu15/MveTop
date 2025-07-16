using System.Collections;
using UnityEngine;

public class BulletSwapm : MonoBehaviour
{
    [SerializeField] public GameObject bulletPrefab;
    [SerializeField] public int bulletCount = 15;
    [SerializeField] public float bulletSpeed = 5f;
    [SerializeField] public float shootCooldown = 5f;

    public bool isShooting = false;
    private bool isOnCooldown = false;

    public event System.Action OnShotBulletFinished;

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

}
