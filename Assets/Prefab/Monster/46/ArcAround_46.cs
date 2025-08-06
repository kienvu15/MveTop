using System.Diagnostics.Tracing;
using UnityEngine;

public class ArcAround_46 : EnemyState
{
    public ArcAround_46(EnemyBrain brain) : base(brain) { }

    public float stateTimer= 0f;
    public float stateDuration = Random.Range(1f, 2f);

    public AvoidPlayer avoidPlayer;
    public BulletSwapm bulletSwapm;
    public EnemyLaserBeam enemyLaserBeam;

    public bool ok = false;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("ArcAround_46: Entering arc around state");
        avoidPlayer = brain.GetComponent<AvoidPlayer>(); 
        bulletSwapm = brain.GetComponent<BulletSwapm>();
        enemyLaserBeam = brain.GetComponent<EnemyLaserBeam>();
        ok = false;
    }

    public override void Update()
    {
        if(brain.EnemyVision.CanSeePlayer)
        {
            stateTimer += Time.deltaTime;
            if (stateTimer < stateDuration)
            {
                if (ok == false)
                {
                    enemyLaserBeam.MoveWhileChargingWithoutPlayer();
                }
                
            }
            else
            {
                brain.EnemySteering.StopMoving();
                bulletSwapm.ShotMissileCondition();
            }
        }
        else
        {
            stateTimer = 0f; // Reset timer if player is not seen
        }

        if(bulletSwapm.isShooting == true)
        {
            brain.ChangeState(new DodgeStage_46(brain));
        }

        if (brain.EnemyVision.lastSeenPosition.HasValue && !brain.EnemyVision.CanSeePlayer)
        {
            brain.EnemySteering.StopMoving();
            ok = true;
            brain.ChangeState(new LastSeenStage_46(brain));
        }

        if(brain.EnemyAttackVision.isPlayerInAttackRange == true)
        {
            ok = true;
            brain.EnemySteering.StopMoving();
            brain.ChangeState(new RetreadStage_46(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

}
