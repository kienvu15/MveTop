using UnityEngine;

public class AttackRange : MonoBehaviour
{
    public float attackRange = 1f; // Khoảng cách tấn công

    public bool isPlayerInRange = false; // Biến để kiểm tra xem Player có trong phạm vi tấn công hay không

    public LayerMask playerLayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckAttack();
    }

    public void CheckAttack()
    {
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, playerLayer);
        if (hit != null)
        {
            isPlayerInRange = true; // Đặt biến là true nếu Player trong phạm vi tấn công
            Debug.Log("Player in attack range");
            // Thực hiện hành động tấn công ở đây
            // Ví dụ: hit.GetComponent<Player>().TakeDamage(damageAmount);
        }
        else
        {
            Debug.Log("No player in attack range");
        }
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
