using UnityEngine;

public class Coin : MonoBehaviour
{
    public Transform Player;
    public int value = 1000;
    public float moveTriggerDistance = 3f;       // Khi nào coin bắt đầu bay về player
    public float collectDistance = 1.5f;          // Khoảng cách để collect
    public float moveSpeed = 2f;                  // Tốc độ ban đầu
    public float acceleration = 5f;               // Tăng tốc theo thời gian

    private Rigidbody2D rb;
    private bool isMovingToPlayer = false;
    private float currentSpeed = 0f;

    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Player == null) return;

        float distance = Vector2.Distance(transform.position, Player.position);

        if (distance < moveTriggerDistance)
        {
            isMovingToPlayer = true;
        }

        if (isMovingToPlayer)
        {
            currentSpeed += acceleration * Time.deltaTime;
            Vector2 direction = (Player.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * currentSpeed * Time.deltaTime);

            if (distance < collectDistance)
            {
                CoinManager.Instance.AddCoin(value);
                Destroy(gameObject);
            }
        }
    }
}
