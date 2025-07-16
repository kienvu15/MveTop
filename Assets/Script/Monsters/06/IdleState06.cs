using UnityEngine;

public class IdleState06 : EnemyState
{
    public IdleState06(EnemyBrain brain) : base(brain) { }

    public override void Enter() { }

    public override void Update()
    {
        if (brain.EnemyVision.CanSeePlayer)
        {
            brain.ChangeState(new DecisionStage_06(brain));
        }
    }

    public override void Exit() { }
}
