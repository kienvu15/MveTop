using UnityEditor.Recorder;
using UnityEngine;
using static EnemySteering;

public class DecisionStage_03 : EnemyState
{
    public DecisionStage_03(EnemyBrain brain) : base(brain) { }

    private EnemyRandomPatrolSteering enemyRandomPatrolSteering;

    public float stateTimer;
    public float stateDuration;

    private bool canCurvedMove = true;

    public override void Enter()
    {
        base.Enter();
        Debug.Log("DecisionStage_03");
        enemyRandomPatrolSteering = brain.GetComponent<EnemyRandomPatrolSteering>();

        stateDuration = 2f;
        stateTimer = 0f;

        enemyRandomPatrolSteering.enabled = true;
        canCurvedMove = true;
    }

    public override void Update()
    {

        if (brain.EnemyVision.CanSeePlayer == true)
        {
            stateTimer += Time.deltaTime;
            if (stateTimer >= stateDuration)
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
                    brain.EnemySteering.MoveToWithBendSmart(brain.EnemyVision.targetDetected.position, brain.EnemySteering.chosenCurveMode, 1.5f);
                }
            }
        }

        if (brain.EnemyVision.lastSeenPosition != null)
        {
            brain.EnemySteering.MoveTo(brain.EnemyVision.lastSeenPosition.Value, 1.5f);
        }

        if(brain.EnemyVision.distance < 3f)
        {
            Debug.Log("DecisionStage_03: Close enough to attack");

            canCurvedMove = false;
            brain.EnemySteering.StopMoving();
            brain.ChangeState(new AttackStage_03(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
