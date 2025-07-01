using UnityEngine;

public class DecisionStage_48 : EnemyState
{
    private EnemyBounceRun EnemyBounceRun;
    private EnemyShot EnemyShot;
    private EnemyDashAttack EnemyDashAttack;

    public DecisionStage_48(EnemyBrain brain) : base(brain){}

    //phase1
    private float RunWeight = 0.6f;
    private float ShotWeight = 0.4f;
    private float originShotWieght = 0.4f;

    //bool
    private bool RunUsed = false;
    private bool ShotUsed = false;

    public enum LastActionType { None, Run, Shot }

    public override void Enter()
    {
        EnemyBounceRun = brain.GetComponent<EnemyBounceRun>();
        EnemyShot = brain.GetComponent<EnemyShot>();
        EnemyDashAttack = brain.GetComponent<EnemyDashAttack>();

        AdjustWeight();
    }
    public override void Update()
    {
        // Mất dấu Player
        if (brain.EnemyVision.hasSeenPlayer && !brain.EnemyVision.CanSeePlayer)
        {
            brain.ChangeState(new SteeringStage(brain, new DecisionStage_48(brain)));
        }

        //Begin
        if (brain.EnemyVision.CanSeePlayer == true && brain.EnemyAttackVision.isPlayerInAttackRange == false)
        {
            float rand = Random.value;
            if (rand < RunWeight)
            {
                brain.ChangeState(new Monster_48_RunStage(brain, new DecisionStage_48(brain, LastActionType.Run)));
            }
            else
            {
                brain.ChangeState(new Monster_48_ShotStage(brain, new DecisionStage_48(brain, LastActionType.Shot)));
            }
        }
    }

    private LastActionType lastAction = LastActionType.None;
    public DecisionStage_48(EnemyBrain brain, LastActionType last) : base(brain)
    {
        lastAction = last;
    }
    void AdjustWeight()
    {
        switch (lastAction)
        {
            case LastActionType.Run:
                RunWeight = 0.3f;
                ShotWeight = 0.7f;
                break;
            case LastActionType.Shot:
                RunWeight = 0.7f;
                ShotWeight = 0.3f;
                break;
            default:
                RunWeight = 0.5f;
                ShotWeight = 0.5f;
                break;
        }
    }

    public override void Exit()
    {

    }
}
 