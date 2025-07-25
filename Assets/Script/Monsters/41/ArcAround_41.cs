using UnityEngine;
using static EnemySteering;

public class ArcAround_41 : EnemyState
{
    public ArcAround_41(EnemyBrain brain) : base(brain) { }

    private bool canCurvedMove = true;

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        if(brain.EnemyVision.CanSeePlayer)
        {
            // Move To Player
            if (!brain.EnemySteering.hasChosenCurve)
            {
                brain.EnemySteering.chosenCurveMode = (CurveMode)UnityEngine.Random.Range(0, 3);
                brain.EnemySteering.hasChosenCurve = true;

                if (brain.EnemySteering.chosenCurveMode == CurveMode.LoopBack)
                    brain.EnemySteering.SetRandomLoopbackOffset();
            }

            if (canCurvedMove)
            {
                Debug.Log("Move to player");
                brain.EnemySteering.MoveToWithBendSmart(brain.EnemyVision.targetDetected.position, brain.EnemySteering.chosenCurveMode, 2f);
            }
        }

        if(brain.EnemyVision.lastSeenPosition.HasValue && !brain.EnemyVision.CanSeePlayer)
        {
            Debug.Log("Move to last seen position");
            brain.EnemySteering.MoveTo(brain.EnemyVision.lastSeenPosition.Value, 1.2f);
        }

        if (brain.EnemyVision.distance < 3.5f)
        {
            brain.EnemySteering.StopMoving();
            brain.ChangeState(new AttackStage_41(brain));
        }
        
    }

    public override void Exit()
    {
        base.Exit();
        // Optionally, you can add any cleanup code here when exiting the arc movement state
    }
}
