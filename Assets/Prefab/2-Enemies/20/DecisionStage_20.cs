using UnityEngine;

public class DecisionStage_20 : EnemyState
{
    public DecisionStage_20(EnemyBrain brain) : base(brain) { }
    public EnemyLaserBeam eneemyLaserBeam;
    public float stateTimer = 0f;
    public float stateDuration = Random.Range(1f, 2f); // Duration for the decision stage
    public override void Enter()
    {
        base.Enter();
        Debug.Log("DecisionStage_20: Entering decision stage");
        // Here you can add any initialization code for the decision stage
        eneemyLaserBeam = brain.GetComponent<EnemyLaserBeam>();
        stateTimer = 0f;
        stateDuration = Random.Range(0.5f, 1f);
    }

    public override void Update()
    {
        if (brain.EnemyStateController.canMove) 
        {
            eneemyLaserBeam.MoveWhileChargingWithoutPlayer();
        }
            
        stateTimer += Time.deltaTime;
        if (stateTimer >= stateDuration)
        {
            if(brain.EnemyVision.CanSeePlayer)
            {
                Debug.Log("DecisionStage_20: Time elapsed, transitioning to MoveStage_20");
                brain.ChangeState(new MoveStage_20(brain));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("DecisionStage_20: Exiting decision stage");
        // Here you can add any cleanup code for the decision stage
    }
}
