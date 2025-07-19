using System.Collections;
using UnityEngine;

public class PlayerCombo : MonoBehaviour
{
    

    private bool isDashingAttack = false;
    private Rigidbody2D rb;
    private PlayerStateController stateController;
    private PlayerInputHandler inputHandler;
    private PlayerAttack attack;
    private PlayerStats playerStats;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateController = GetComponent<PlayerStateController>();
        inputHandler = FindFirstObjectByType<PlayerInputHandler>();
        attack = GetComponent<PlayerAttack>();
        playerStats = GetComponent<PlayerStats>();
    }

    public bool CanAttackNow()
    {
        return inputHandler.AttackPressed && stateController.canAttack && !isDashingAttack;
    }

    public void NextComboStep()
    {
        playerStats.comboStep++;
        if (playerStats.comboStep > 3)
            playerStats.comboStep = 1;
    }

    public void TryDashAttack()
    {
        if (playerStats.comboStep == 3)
            StartCoroutine(DashAttack(playerStats.finalDashSpeed));
        else
            StartCoroutine(DashAttack(playerStats.dashOnHitSpeed));
    }

    private IEnumerator DashAttack(float speed)
    {
        isDashingAttack = true;
        stateController.isDashing = true;

        Vector2 dashDir = (attack.attackPoint.position - transform.position).normalized;
        float timer = 0f;

        while (timer < playerStats.attackdashDuration)
        {
            rb.linearVelocity = dashDir * speed;
            timer += Time.deltaTime;
            yield return null;
        }

        rb.linearVelocity = Vector2.zero;
        isDashingAttack = false;
        stateController.isDashing = false;
    }
}
