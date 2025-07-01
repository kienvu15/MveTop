using UnityEngine;

public class EnemyCharge : MonoBehaviour
{
    public float chargeDelay = 1f;        // th?i gian v?n công
    public float chargeSpeed = 10f;       // t?c ?? lao t?i
    public float chargeDuration = 0.5f;   // th?i gian lao

    private Vector2 direction;
    private float timer;
    private bool isCharging = false;
    private bool isWindingUp = false;

    private Rigidbody2D rb;

    public System.Action OnChargeComplete; // callback khi xong

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void StartCharge(Vector2 dir)
    {
        direction = dir.normalized;
        isWindingUp = true;
        timer = 0f;
    }

    private void FixedUpdate()
    {
        if (isWindingUp)
        {
            timer += Time.fixedDeltaTime;
            if (timer >= chargeDelay)
            {
                timer = 0f;
                isWindingUp = false;
                isCharging = true;
            }
        }
        else if (isCharging)
        {
            rb.linearVelocity = direction * chargeSpeed;
            timer += Time.fixedDeltaTime;
            if (timer >= chargeDuration)
            {
                rb.linearVelocity = Vector2.zero;
                isCharging = false;
                OnChargeComplete?.Invoke(); // báo v? cho state
            }
        }
    }

    public bool IsCharging => isCharging || isWindingUp;

    public void StopCharge()
    {
        isCharging = false;
        isWindingUp = false;
        rb.linearVelocity = Vector2.zero;
    }
}
