using UnityEngine;

public class EnemyTypeSetup : MonoBehaviour
{
    public enum EnemyType { Type01, Type02, Type06, Type48 }

    public EnemyType type;

    private void Awake()
    {
        var brain = GetComponent<EnemyBrain>();
        if (brain == null) return;

        switch (type)
        {
            case EnemyType.Type01:
                brain.GetDecisionStageFunc = () => new IdleState(brain);
                break;

            case EnemyType.Type02:
                brain.GetDecisionStageFunc = () => new IdleState(brain);
                break;

            case EnemyType.Type06:
                brain.GetDecisionStageFunc = () => new DecisionStage_06(brain);
                break;

            case EnemyType.Type48:
                brain.GetDecisionStageFunc = () => new DecisionStage_48(brain);
                break;
        }
    }
}
