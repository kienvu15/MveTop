using UnityEngine;

public class LastSeenStage_20 : EnemyState
{
    public LastSeenStage_20(EnemyBrain brain) : base(brain) { }

    public AvoidPlayer avoidPlayer;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("LastSeenStage_20: Entering last seen stage");
        // Here you can add any initialization code for the last seen stage
        avoidPlayer = brain.GetComponent<AvoidPlayer>();
    }

    public override void Update()
    {
        if (!brain.EnemyVision.CanSeePlayer && brain.EnemyVision.lastSeenPosition.HasValue)
        {
            Vector2 lastSeen = brain.EnemyVision.lastSeenPosition.Value;
            brain.EnemySteering.MoveTo(lastSeen, 2f);

            float dist = Vector2.Distance(brain.transform.position, lastSeen);
            if (dist < avoidPlayer.nodeStopDistance)
            {
                brain.EnemySteering.StopMoving();
                brain.ChangeState(new DecisionStage_20(brain));
            }
        }

        if(brain.EnemyVision.CanSeePlayer)
        {
            brain.EnemySteering.StopMoving();
            Debug.Log("LastSeenStage_20: Player spotted, transitioning to MoveStage_20");
            brain.ChangeState(new MoveStage_20(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("LastSeenStage_20: Exiting last seen stage");
        // Here you can add any cleanup code for the last seen stage
    }
}
