using UnityEngine;

public class AttackStage_41 : EnemyState
{
    public AttackStage_41(EnemyBrain brain) : base(brain) { }

    private float randChoice;
    private float stateTimer;
    private float stateDuration;

    private RangedEnemyController rangedEnemyController;
    private EnemyAttackController enemyAttackController;    

    public override void Enter()
    {
        base.Enter();
        Debug.Log("AttackStage_41: Entering attack stage");
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
        enemyAttackController = brain.GetComponent<EnemyAttackController>();
        randChoice = Random.value;

        stateTimer = 0f;
        stateDuration = Random.Range(1f, 2f);
        rangedEnemyController.isOrbitingPlayer = true;
    }

    public override void Update()
    {

       if(randChoice > 0.5f)
        {
            if (brain.EnemyStateController.canMove)
            {
                rangedEnemyController.DritDec();
            }

            stateTimer += Time.deltaTime;
            
            if (stateTimer >= stateDuration)
            {
                rangedEnemyController.StopDritDec();
                enemyAttackController.isLocking = true;
                brain.EnemyAttackVision.isAttackLocked = true;
                brain.ChangeState(new RetreadStage_41(brain));
            }
        }

        else
        {
            if (brain.EnemyStateController.canMove)
            {
                rangedEnemyController.ArcAroundPlayer();
            }
            
            stateTimer += Time.deltaTime;
            if (stateTimer >= 1f)
            {
                rangedEnemyController.isOrbitingPlayer = false;
                enemyAttackController.isLocking = true;
                brain.EnemyAttackVision.isAttackLocked = true;
                brain.ChangeState(new RetreadStage_41(brain));
            }
        }

    }

    public override void Exit()
    {
        base.Exit();
        
    }
}
