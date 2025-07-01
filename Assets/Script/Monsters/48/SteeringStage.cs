using UnityEngine;

public class SteeringStage : EnemyState
{
    public SteeringLastScenePosition SteeringLastScenePosition;
    public EnemyVision EnemyVision;
    public SteeringStage(EnemyBrain brain, EnemyState nextStage) : base(brain, nextStage)
    {
        this.nextStage = nextStage;
    }

    public override void Enter()
    {
        base.Enter();
        SteeringLastScenePosition = brain.GetComponent<SteeringLastScenePosition>();
        EnemyVision = brain.GetComponent<EnemyVision>();
        SteeringLastScenePosition.OnSteeringFinished += FinishStage;
    }
 
    public override void Update()
    {
        if (EnemyVision.CanSeePlayer)
        {
            brain.ChangeState(new DecisionStage_48(brain));
            return;
        }
        else if (EnemyVision.lastSeenPosition.HasValue)
        {
            SteeringLastScenePosition.GoToLastSeenPosition();
        }
        
        if (IsFinished)
            brain.ChangeState(nextStage);
    }

    public override void Exit()
    {
        SteeringLastScenePosition.OnSteeringFinished -= FinishStage;
    }
    public void FinishStage() => IsFinished = true;
}
