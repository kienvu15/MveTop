using UnityEngine;

public class RetreadStage_03 : EnemyState
{

    public RetreadStage_03(EnemyBrain brain) : base(brain) { }
    private Retread retread;

    private float stateTimer;
    private float stateDuration;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("RetreadStage_03: Entering Retread stage");
        retread = brain.GetComponent<Retread>();
        retread.isDone = false; // Reset the retread state

        stateTimer = 0f;
        stateDuration = Random.Range(1f, 1.2f);

    }
    public override void Update()
    {
        if (brain.EnemyVision.targetDetected != null)
        {
            retread.RetreatIfCloseTo(brain.EnemyVision.targetDetected, retreatThreshold: 2f, retreatDistance: 3f, retreatSpeed: 3f);
        }

        stateTimer += Time.deltaTime;
        if (stateTimer >= stateDuration)
        {
            brain.EnemySteering.StopMoving();
            Debug.Log("Retreat stage timed out, switching to Decision stage");
            brain.ChangeState(new DecisionStage_03(brain));
        }
    }
    public override void Exit()
    {
        base.Exit();
        Debug.Log("RetreadStage_03: Exiting Retread stage");
    }
}
