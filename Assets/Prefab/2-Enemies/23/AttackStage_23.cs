using UnityEngine;

public class AttackStage_23 : EnemyState
{
    public AttackStage_23(EnemyBrain brain) : base(brain) { }

    private RangedEnemyController rangedEnemyController;


    private float stateTimer;
    private float stateTimer02;
    private float stateDuration;
    private float stateDuration02;

    private bool GoingToAttack = false;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("AttackStage_23: Entering attack state");
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
        // Here you can add any initialization code for the attack stage
        GoingToAttack = false;

        stateTimer = 0f;
        stateTimer02 = 0f;
        stateDuration = Random.Range(0.3f, 0.4f);
        stateDuration02 = Random.Range(0.4f, 0.7f);
    }

    public override void Update()
    {
        if (brain.EnemyAttackVision.isPlayerInAttackRange == true)
        {
            if (brain.EnemyStateController.canMove)
            {
                rangedEnemyController.DritDec();
            }
            stateTimer02 += Time.deltaTime;
            if (stateTimer02 > stateDuration02)
            {
                Debug.Log("AttackStage_23: Player in attack range, go performing attack");
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
                brain.ChangeState(new DecisionStage_23(brain));
            }
        }

        if (GoingToAttack == true)
        {
            rangedEnemyController.StopDritDec();
            brain.ChangeState(new EnganeStage_23(brain));
        }

        if (brain.EnemyVision.lastSeenPosition != null && brain.EnemyVision.CanSeePlayer == false)
        {
            Vector2 lastSeen = brain.EnemyVision.lastSeenPosition.Value;
            rangedEnemyController.StopDritDec();
            brain.EnemySteering.MoveTo(lastSeen, 2f);

            float dist = Vector2.Distance(brain.transform.position, lastSeen);
            if (dist < 0.5f)
            {
                brain.EnemySteering.StopMoving();
                brain.ChangeState(new PatrolStage_03(brain));
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        Debug.Log("AttackStage_23: Exiting attack state");
        // Here you can add any cleanup code for the attack stage
    }

}
