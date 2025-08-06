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

        if (brain.EnemyVision.CanSeePlayer == true && brain.EnemyStateController.canMove)
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
                    brain.EnemySteering.MoveToWithBendSmart(brain.EnemyVision.targetDetected.position, brain.EnemySteering.chosenCurveMode, 3.5f);
                }
            }
        }

        if (brain.EnemyVision.lastSeenPosition != null)
        {
            Vector2 lastSeen = brain.EnemyVision.lastSeenPosition.Value;
            brain.EnemySteering.MoveTo(lastSeen, 2f);

            float dist = Vector2.Distance(brain.transform.position, lastSeen);
            if (dist < 0.5f)
            {
                brain.EnemySteering.StopMoving();
                brain.ChangeState(new PatrolStage_03(brain));
            }   
        }

        if(brain.EnemyVision.distance < 3.4f)
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
