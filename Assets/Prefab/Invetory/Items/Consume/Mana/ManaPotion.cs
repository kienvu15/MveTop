using UnityEngine;

public class ManaPotion : MonoBehaviour
{

    public enum ManaType
    {
        SmallMana,
        MediumMana,
        BigMana,
    }

    public ManaType value;

    public float moveTriggerDistance = 3f;
    public float collectDistance = 1.5f;
    public float moveSpeed = 2f;
    public float acceleration = 5f;

    private Rigidbody2D rb;
    public Transform Player;

    private bool isMovingToPlayer = false;
    private float currentSpeed = 0f;

    private int manaAmount;

    private void Awake()
    {
        // Gán giá trị coinAmount theo loại coin
        switch (value)
        {
            case ManaType.SmallMana:
                manaAmount = Random.Range(1, 6); // 1–5
                break;
            case ManaType.MediumMana:
                manaAmount = Random.Range(6, 11); // 6–10
                break;
            case ManaType.BigMana:
                manaAmount = Random.Range(11, 21); // 11–20
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

        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats playerStats = other.GetComponent<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.RestoreMana(manaAmount);
                Destroy(gameObject); // Xoá item sau khi ăn
            }
        }
    }
}
