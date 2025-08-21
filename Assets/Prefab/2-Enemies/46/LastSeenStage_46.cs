using UnityEngine;

public class LastSeenStage_46 : EnemyState
{
    public LastSeenStage_46(EnemyBrain brain) : base(brain) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log("LastSeenStage_46: Entering last seen stage");
    }

    public override void Update()
    {
        if (!brain.EnemyVision.CanSeePlayer && brain.EnemyVision.lastSeenPosition.HasValue)
        {
            Vector2 lastSeen = brain.EnemyVision.lastSeenPosition.Value;
            brain.EnemySteering.MoveTo(lastSeen, 2f);

            float dist = Vector2.Distance(brain.transform.position, lastSeen);
            if (dist < 0.5f)
            {
                brain.EnemySteering.StopMoving();
                brain.ChangeState(new DecisionStage_46(brain));
            }
        }

        if (brain.EnemyVision.CanSeePlayer)
        {
            brain.EnemySteering.StopMoving();
            Debug.Log("LastSeenStage_20: Player spotted, transitioning to MoveStage_20");
            brain.ChangeState(new ArcAround_46(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
        // Here you can add any cleanup code for the last seen stage
        Debug.Log("LastSeenStage_46: Exiting last seen stage");
    }
}
