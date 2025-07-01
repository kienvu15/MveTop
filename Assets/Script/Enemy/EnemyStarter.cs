using UnityEngine;

public enum EnemyType { Melee, Ranged, Sword, Arrow }

public class EnemyStarter : MonoBehaviour
{
    public EnemyType type;
    private EnemyStateMachine fsm;

    void Start()
    {
        fsm = GetComponent<EnemyStateMachine>();
        fsm.player = GameObject.FindWithTag("Player").transform;

        // Chọn đúng trạng thái đầu tiên dựa theo loại
        switch (type)
        {
            case EnemyType.Melee:
                fsm.Init(new Wander());
                break;

            case EnemyType.Ranged:
                fsm.Init(new RangedIdleState());
                break;

            case EnemyType.Sword:
                fsm.Init(new SwordIdleState());
                break;
        }
    }
}
