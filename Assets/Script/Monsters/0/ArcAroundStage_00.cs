using UnityEngine;

public class ArcAroundStage_00 : EnemyState
{
    public ArcAroundStage_00(EnemyBrain brain) : base(brain) { }
    
    private RangedEnemyController rangedEnemyController;
    private BulletSwapm bulletSwapm;

    private float startTime;
    private float duration; // Duration for the arc around stage
    public override void Enter()
    {
        base.Enter();
        Debug.Log("ArcAroundStage_00: Entering arc around stage");
        // Here you can add any initialization code for the arc around stage
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
        bulletSwapm = brain.GetComponent<BulletSwapm>();

        startTime = 0f;
        duration = Random.Range(1.5f, 5f); // Random duration for the arc around stage
    }
    public override void Update()
    {
        if(brain.EnemyVision.CanSeePlayer)
        {
            rangedEnemyController.DritDec();
            bulletSwapm.ShotGunCondition();
            startTime += Time.deltaTime;
            if(startTime >= duration)
            {
                rangedEnemyController.StopDritDec();
                brain.ChangeState(new Move_RetreadStage_00(brain));
            }
        }

        if (brain.EnemyVision.lastSeenPosition.HasValue)
        {
            brain.EnemySteering.MoveTo(brain.EnemyVision.lastSeenPosition.Value, 1.6f);
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("ArcAroundStage_00: Exiting arc around stage");
        // Here you can add any cleanup code for the arc around stage
    }
}
