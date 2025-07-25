using UnityEngine;

public class EnemyTypeSetup : MonoBehaviour
{
    public enum EnemyType { Type00 ,Type01, Type02, Type03, Type06, Type41, Type48 }

    public EnemyType type;

    private void Awake()
    {
        var brain = GetComponent<EnemyBrain>();
        if (brain == null) return;

        switch (type)
        {
            case EnemyType.Type00:
                brain.GetDecisionStageFunc = () => new DecisionStage_00(brain);
                break;

            case EnemyType.Type01:
                brain.GetDecisionStageFunc = () => new IdleState(brain);
                break;

            case EnemyType.Type02:
                brain.GetDecisionStageFunc = () => new IdleState(brain);
                break;

            case EnemyType.Type03:
                brain.GetDecisionStageFunc = () => new PatrolStage_03(brain);
                break;

            case EnemyType.Type06:
                brain.GetDecisionStageFunc = () => new DecisionStage_06(brain);
                break;

            case EnemyType.Type41:
                brain.GetDecisionStageFunc = () => new DecisionStage_41(brain);
                break;


            case EnemyType.Type48:
                brain.GetDecisionStageFunc = () => new DecisionStage_48(brain);
                break;
        }
    }
}
