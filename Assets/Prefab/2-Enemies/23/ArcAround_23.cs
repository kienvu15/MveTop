using UnityEngine;
using static EnemySteering;

public class ArcAround_23 : EnemyState
{
    public ArcAround_23(EnemyBrain brain) : base(brain) { }

    private bool canCurvedMove = true;

    private float random = Random.value;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("ArcAround_23");
        canCurvedMove = true;
    }

    public override void Update()
    {
        if (random < 0.5f)
        {
            if (brain.EnemyVision.CanSeePlayer == true && brain.EnemyStateController.canMove)
            {

                Debug.Log("Move To Player");
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
                    brain.EnemySteering.MoveToWithBendSmart(brain.EnemyVision.targetDetected.position, brain.EnemySteering.chosenCurveMode, 3.3f);
                }

            }
        }
        else
        {
            brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 3.3f);

            if(brain.EnemyVision.targetDetected == null)
            {
                Vector2 lastSeen = brain.EnemyVision.lastSeenPosition.Value;
                brain.EnemySteering.MoveTo(lastSeen, 1f);

                float dist = Vector2.Distance(brain.transform.position, lastSeen);
                if (dist < 0.5f)
                {
                    brain.EnemySteering.StopMoving();
                    brain.ChangeState(new DecisionStage_23(brain));
                }
            }
        }


        if (brain.EnemyVision.lastSeenPosition != null && brain.EnemyVision.CanSeePlayer == false)
        {
            Vector2 lastSeen = brain.EnemyVision.lastSeenPosition.Value;
            brain.EnemySteering.MoveTo(lastSeen, 1f);

            float dist = Vector2.Distance(brain.transform.position, lastSeen);
            if (dist < 0.5f)
            {
                brain.EnemySteering.StopMoving();
                brain.ChangeState(new DecisionStage_23(brain));
            }
        }

        if (brain.EnemyVision.distance < 3.5f)
        {
            Debug.Log("DecisionStage_23: Close enough to attack");

            canCurvedMove = false;
            brain.EnemySteering.StopMoving();
            brain.ChangeState(new AttackStage_23(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}