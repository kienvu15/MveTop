using System.Collections;
using UnityEngine;

public class ItmEfftecTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyStats enemy = collision.GetComponent<EnemyStats>();
        if (enemy != null)
        {
            enemy.recoilHitForce = 1.2f;
            enemy.TakeDamage(0f, transform.position);

            // Gọi hàm reset sau 0.2 giây
            StartCoroutine(ResetRecoil(enemy, 1.5f));
        }
    }

    private IEnumerator ResetRecoil(EnemyStats enemy, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (enemy != null) // Kiểm tra enemy còn tồn tại không
            enemy.recoilHitForce = enemy.recoilForceOriginal;
    }

}
