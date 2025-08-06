using UnityEngine;

public class PlayerStateController : MonoBehaviour
{
    public bool canMove = true;
    public bool canFlip = true;
    public bool canDash = true;
    public bool canAttack = true;
    public bool hurt = false;

    public bool isDashing = false;
    public bool isRecoiling = false;
}
