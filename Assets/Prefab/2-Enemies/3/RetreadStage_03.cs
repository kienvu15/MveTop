using UnityEngine;

public class RetreadStage_03 : EnemyState
{

    public RetreadStage_03(EnemyBrain brain) : base(brain) { }
    private Retread retread;

    private float stateTimer;
    private float stateDuration;

    private float random = Random.value;
    private float random2 = Random.value;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("RetreadStage_03: Entering Retread stage");
        retread = brain.GetComponent<Retread>();
        retread.isDone = false; // Reset the retread state

        stateTimer = 0f;
        stateDuration = Random.Range(0.6f, 1f);

    }
    public override void Update()
    {
        if(random2 < 0.5f)
        {
            if (brain.EnemyVision.targetDetected != null)
            {
                retread.RetreatIfCloseTo(brain.EnemyVision.targetDetected, retreatThreshold: 2f, retreatDistance: 3f, retreatSpeed: 3f);
            }
        }
        else if(random2 > 0.5f)
        {
            brain.ChangeState(new EnganeStage_03(brain));
        }
        

        stateTimer += Time.deltaTime;
        if (stateTimer >= stateDuration)
        {
            if(random < 0.5f)
            {
                brain.EnemySteering.StopMoving();
                Debug.Log("Retreat stage timed out, switching to Decision stage");
                brain.ChangeState(new DecisionStage_03(brain));
            }
            else
            {
                brain.EnemySteering.StopMoving();
                Debug.Log("Retreat stage timed out, switching to Engane stage");
                brain.ChangeState(new EnganeStage_03(brain));
            }
            
        }
    }
    public override void Exit()
    {
        base.Exit();
        Debug.Log("RetreadStage_03: Exiting Retread stage");
    }
}
