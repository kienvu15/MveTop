using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector2 Movement { get; private set; }
    public float moveSpeed = 5f;

    private PlayerStateController StateController;
    private Rigidbody2D rb;
    private PlayerInputHandler input;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInputHandler>();
        StateController = GetComponent<PlayerStateController>();
    }

    void FixedUpdate()
    {
        if (!StateController.isDashing && !StateController.isRecoiling)
        {
            Movement = input.MoveInput;
            Movement.Normalize();
            rb.linearVelocity = Movement * moveSpeed;
        } 
    }

}