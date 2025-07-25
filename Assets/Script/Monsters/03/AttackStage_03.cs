using UnityEditor.Recorder;
using UnityEngine;

public class AttackStage_03 : EnemyState
{
    public AttackStage_03(EnemyBrain brain) : base(brain) { }

    private RangedEnemyController rangedEnemyController;
    private EnemyAttackController enemyAttackController;
    private Retread retread;

    private float stateTimer;
    private float stateTimer02;
    private float stateDuration;
    private float stateDuration02;

    private bool approachingPlayer = false;
    private bool isStioping = false;

    private bool GoingToAttack = false;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("AttackStage_03: Entering attack state");
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
        enemyAttackController = brain.GetComponent<EnemyAttackController>();
        retread = brain.GetComponent<Retread>();
        // Here you can add any initialization code for the attack stage
        GoingToAttack = false;

        stateTimer = 0f;
        stateTimer02 = 0f;
        stateDuration = Random.Range(1f, 1.2f); 
        stateDuration02 = Random.Range(0.7f, 2f);
    }

    public override void Update()
    {
        if (brain.EnemyAttackVision.isPlayerInAttackRange == true)
        {
            rangedEnemyController.DritDec();
            stateTimer02 += Time.deltaTime;
            if (stateTimer02 > stateDuration02)
            {
                Debug.Log("AttackStage_03: Player in attack range, go performing attack");
                GoingToAttack = true;
                stateTimer02 = 0f; // Reset timer after attack
            }
        }
        else
        {
            stateTimer += Time.deltaTime;
            if (stateTimer > stateDuration)
            {
                rangedEnemyController.StopDritDec();
                brain.ChangeState(new DecisionStage_03(brain));
            }
        }

        if (GoingToAttack == true)
        {
            rangedEnemyController.StopDritDec();
            brain.ChangeState(new EnganeStage_03(brain));
        }

        //if(isStioping == true)
        //{
        //    brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 1.5f);
        //    if(brain.EnemyVision.distance < 1.6f)
        //    {
        //        brain.EnemySteering.StopMoving();
        //        enemyAttackController.TryPerformAttack();
        //    }
        //}
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("AttackStage_03: Exiting attack state");
        // Here you can add any cleanup code for the attack stage
    }

}
