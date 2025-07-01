using UnityEngine;
using UnityEngine.Rendering;

public class IdleState : EnemyState
{
    public IdleState(EnemyBrain brain) : base(brain){ }

    public override void Enter() { }

    public override void Update()
    {
        if (brain.EnemyVision.CanSeePlayer)
        {
            brain.ChangeState(new DecisionStage_48(brain));
        }
    }

    public override void Exit() { }
}
