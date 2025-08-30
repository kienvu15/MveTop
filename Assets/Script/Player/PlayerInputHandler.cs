using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }
    public bool DashPressed { get; private set; }
    public bool AttackPressed { get; private set; }

    public bool isLocked = false;
    void Update()
    {
        if (isLocked)
        {
            MoveInput = Vector2.zero;
            DashPressed = false;
            AttackPressed = false;
            return;
        }

        MoveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        DashPressed = Input.GetKeyDown(KeyCode.Space);
        AttackPressed = Input.GetKeyDown(KeyCode.V);
    }

    public void ResetDashInput() => DashPressed = false;
    public void ResetMoveInput() => MoveInput = Vector2.zero;
    public void ResetAttackInput() => AttackPressed = false;
}
