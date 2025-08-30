using System.Collections;
using UnityEngine;

public class BlueForce : MonoBehaviour
{
    //public PlayerStateController playerStateController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      //  playerStateController = FindFirstObjectByType<PlayerStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveCollider()
    { 
        GetComponent<PolygonCollider2D>().enabled = true;
    }


    public void DestoyOnLastFrame()
    {
        gameObject.SetActive(false);
        //playerStateController.hurt = false;
    }

    public void ĐeDestroy()
    {
        Destroy(gameObject);
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
