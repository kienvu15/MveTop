using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum CoinType
    {
        SmallCoin,
        MediumCoin,
        BigCoin,
        FullofCoin
    }

    public CoinType value;

    public float moveTriggerDistance = 3f;
    public float collectDistance = 1.5f;
    public float moveSpeed = 2f;
    public float acceleration = 5f;

    private Rigidbody2D rb;
    public Transform Player;
    private bool isMovingToPlayer = false;
    private float currentSpeed = 0f;

    private int coinAmount;

    private void Awake()
    {
        // Gán giá trị coinAmount theo loại coin
        switch (value)
        {
            case CoinType.SmallCoin:
                coinAmount = Random.Range(1, 6); // 1–5
                break;
            case CoinType.MediumCoin:
                coinAmount = Random.Range(6, 11); // 6–10
                break;
            case CoinType.BigCoin:
                coinAmount = Random.Range(11, 21); // 11–20
                break;
            case CoinType.FullofCoin:
                coinAmount = Random.Range(30, 51); // 30–50
                break;
        }
    }

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
                CoinManager.Instance.AddCoin(coinAmount); // dùng coinAmount thay vì enum
                Destroy(gameObject);
            }
        }
    }
}
