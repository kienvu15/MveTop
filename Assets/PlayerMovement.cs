using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 Movement { get; private set; }

    private PlayerStateController StateController;
    private Rigidbody2D rb;
    private PlayerInputHandler input;
    private PlayerStats playerStats;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        StateController = GetComponent<PlayerStateController>();
        playerStats = GetComponent<PlayerStats>();
    }

    void FixedUpdate()
    {
        if (!StateController.canMove)
            return;

        if (!StateController.isDashing && !StateController.isRecoiling && StateController.canMove)
        {
            Movement = input.MoveInput;
            Movement.Normalize();
            rb.linearVelocity = Movement * playerStats.moveSpeed;
        } 
    }

}