using System.Collections;
using UnityEngine;

public class ChargeAttack : MonoBehaviour
{
    [Header("Charge Attack")]
    public float chargeTime = 1f;
    public float chargeSpeed = 6f;
    public float chargeCooldown = 3f;
    public float chargeDistance = 8f;

    private Rigidbody2D rb;

    private Vector2 chargeTarget;

    private bool isChargingUp = false;
    private bool isCharging = false;
    public bool canCharge = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator StartCharge(Vector2 direc)
    {

        
        isChargingUp = true;
        rb.linearVelocity = Vector2.zero;

        Vector2 chargeDir = ((Vector2)direc - (Vector2)transform.position).normalized;
        chargeTarget = (Vector2)transform.position + chargeDir * chargeDistance;

        Debug.Log("⚡ Chuẩn bị charge...");
        yield return new WaitForSeconds(chargeTime);

        isChargingUp = false;
        isCharging = true;
        Debug.Log("💥 Đang charge!");

        float chargeDuration = 0.5f;
        float elapsed = 0f;

        while (elapsed < chargeDuration)
        {
            Vector2 dir = (chargeTarget - (Vector2)transform.position).normalized;
            rb.linearVelocity = dir * chargeSpeed;

            elapsed += Time.deltaTime;
            yield return null;
        }

        isCharging = false;

        rb.linearVelocity = Vector2.zero;
        canCharge = false;

        Debug.Log("😤 Hết charge. Đợi cooldown...");
    }



}
