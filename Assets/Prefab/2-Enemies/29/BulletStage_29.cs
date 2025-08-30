using UnityEngine;

public class BulletStage_29 : EnemyState
{
    public BulletStage_29(EnemyBrain brain) : base(brain) { }
    private EnemyLaserBeam enemyLaserBeam;
    private BulletSwapm bulletSwapm;
    private float time = 0f;
    public float duration = 1.5f;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("BulletStage_29: Entering BulletStage state");
        enemyLaserBeam = brain.GetComponent<EnemyLaserBeam>();
        bulletSwapm = brain.GetComponent<BulletSwapm>();
    }

    public override void Update()
    {
        enemyLaserBeam.MoveWhileChargingWithoutPlayer();
        time += Time.deltaTime;
        if (time <= duration)
        {
            bulletSwapm.ShotFourDirectionsCondition();
        }
        else
        {
            brain.ChangeState(new DecisionStage_29(brain));
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("BulletStage_29: Exiting BulletStage state");

    }
}
