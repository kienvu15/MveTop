using UnityEngine;

public class ShotStagetest : EnemyState
{
    public ShotStagetest(EnemyBrain brain, EnemyState nextStage) : base(brain, nextStage)
    {
        this.nextStage = nextStage;
    }

    private float mainChoice;
    private float randChoice;
    private float randChoice2;
    private float randChoice3;
    private float randChoice4;

    private float stateTimer = 0f;
    private float stateTimer2 = 0f;
    private float stateTimer3 = 0f;
    private float stateTimer4 = 0f;
    private float stateTimer5 = 0f;
    private float stateTimer6 = 0f;
    private float wobbleTimer = 0f;
    private float dirTimer = 0f;

    private float retreatWait = 0f;

    private float randomDuration;
    private float randomDuration2;
    private float randomDuration3;
    private float smaillDuration;
    private float randomDistance;

    private bool WoobleInPlace = false;
    private bool hasShotting = false;

    private bool hasRetreatedInThisStage = false;
    private bool hasMovebackThisState = false;

    private BulletSwapm bulletSwapm;
    private RangedEnemyController rangedEnemyController;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Shot stage");
        hasRetreatedInThisStage = false;
        hasMovebackThisState = false;
        hasShotting = false;

        WoobleInPlace = false;
        bulletSwapm = brain.GetComponent<BulletSwapm>();
        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
        bulletSwapm.OnShotBulletFinished += FinishStage;

        randomDuration = Random.Range(2f, 3f);
        randomDuration2 = Random.Range(0.5f, 2.5f);
        smaillDuration = Random.Range(0.4f, 1.3f);
        randomDuration3 = Random.Range(2f, 4f);
        randomDistance = Random.Range(3f, 4.6f);

        stateTimer = 0f;
        stateTimer2 = 0f;
        stateTimer3 = 0f;
        stateTimer4 = 0f;
        stateTimer5 = 0f;
        stateTimer6 = 0f;
        retreatWait = 0f;
        wobbleTimer = 0f;
        dirTimer = 0f;

        randChoice = Random.value;
        randChoice2 = Random.value;
        randChoice3 = Random.value;
        randChoice4 = Random.value;
        mainChoice = Random.value;
    }

    public override void Update()
    {
        if (mainChoice < 0.5f)
        {
            Debug.Log("main 1");

            if (randChoice4 < 0.5f)
            {
                if (WoobleInPlace == false)
                {
                    Debug.Log("Khoảng cách oke bắt đầu Wobb");
                    WoobleInPlace = true;
                    wobbleTimer = 0f;
                    brain.EnemySteering.StartWobbleInPlace(duration: randomDuration2, radius: 0.4f, speed: 1.2f);
                }
                else
                {
                    wobbleTimer += Time.deltaTime;
                    if (wobbleTimer >= smaillDuration)
                    {
                        Debug.Log("Khoảng cách oke bắt đầu Wobb fininsh bắn và đổi stata");
                        bulletSwapm.ShotCondition();
                        if (IsFinished)
                        {
                            //brain.ChangeState(new DecisionStage_06(brain, DecisionStage_06.LastActionType.None));
                        }
                    }
                }
            }
            ///
            else
            {
                if (hasMovebackThisState == false && brain.EnemyVision.distance < randomDistance)
                {
                    Debug.Log("Bắn");
                    bulletSwapm.ShotCondition();
                    if(bulletSwapm.isShooting == false)
                    {
                        Debug.Log("rút ui");
                        hasMovebackThisState = true;
                        wobbleTimer += Time.deltaTime;
                        //brain.EnemySteering.RetreatIfCloseTo(brain.EnemyVision.targetDetected);
                    }

                    if (hasMovebackThisState == true && wobbleTimer >= 1f)
                    {

                        brain.EnemySteering.StartWobbleInPlace(duration: randomDuration, radius: 1f, speed: 1.2f);
                        retreatWait += Time.deltaTime;
                        if (retreatWait >= randomDuration)
                        {
                            dirTimer += Time.deltaTime;
                            if (dirTimer <= randomDuration2)
                            {
                                bulletSwapm.ShotCondition();
                                rangedEnemyController.DritDec();
                            }
                            else
                            {
                                rangedEnemyController.StopDritDec();
                               // brain.ChangeState(new movestatetest(brain, new ShotStagetest(brain, null)));
                            }
                        }
                    }

                }
            }
        }
        else
        {
           // brain.ChangeState(new DecisionStage_06(brain, DecisionStage_06.LastActionType.None));
        }

        ///////////

        //else
        //{

        //    if (randChoice < 0.5f)
        //    {
        //        hasRetreatedInThisStage = true;
        //        rangedEnemyController.RetreatCondition();

        //        if (WoobleInPlace == false)
        //        {
        //            WoobleInPlace = true;
        //            wobbleTimer = 0f;
        //            brain.EnemySteering.StartWobbleInPlace(duration: randomDuration2, radius: 0.4f, speed: 1.2f);
        //        }
        //        else
        //        {
        //            wobbleTimer += Time.deltaTime;
        //            if (wobbleTimer <= randomDuration2)
        //            {
        //                bulletSwapm.ShotCondition();
        //                rangedEnemyController.DritDec();

        //                stateTimer += Time.deltaTime;
        //                if (stateTimer >= randomDuration)
        //                {
        //                    rangedEnemyController.StopDritDec();
        //                    if (randChoice2 <= 1f / 4f)
        //                    {
        //                        brain.ChangeState(new DirtCon(brain, new ShotStagetest(brain, null)));
        //                    }
        //                    else if (randChoice2 <= 2f / 4f)
        //                    {
        //                        brain.ChangeState(new movestatetest(brain, new ShotStagetest(brain, null)));
        //                    }
        //                    else if (randChoice2 <= 3f / 4f)
        //                    {
        //                        brain.ChangeState(new DecisionStage_06(brain, DecisionStage_06.LastActionType.None));
        //                    }
        //                    else
        //                    {
        //                        bulletSwapm.ShotCondition();
        //                        if (IsFinished)
        //                        {
        //                            brain.ChangeState(new DecisionStage_06(brain, DecisionStage_06.LastActionType.None));
        //                        }
        //                    }
        //                }

        //            }
        //        }
        //    }
        //    else
        //    {
        //        stateTimer2 += Time.deltaTime;
        //        if (stateTimer2 <= randomDuration)
        //        {
        //            if (WoobleInPlace == false)
        //            {
        //                WoobleInPlace = true;
        //                wobbleTimer = 0f;
        //                brain.EnemySteering.StartWobbleInPlace(duration: randomDuration2, radius: 0.4f, speed: 1.2f);
        //            }
        //            else
        //            {
        //                wobbleTimer += Time.deltaTime;
        //                if (wobbleTimer >= smaillDuration)
        //                { 
        //                    bulletSwapm.ShotCondition();
        //                    if (IsFinished)
        //                    {
        //                        hasRetreatedInThisStage = true;
        //                        rangedEnemyController.RetreatCondition();
        //                        stateTimer5 += Time.deltaTime;
        //                        if(stateTimer5 >= smaillDuration)
        //                        {
        //                            brain.ChangeState(new DirtCon(brain, new ShotStagetest(brain, null)));
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        else
        //        {
        //            stateTimer6 += Time.deltaTime;
        //            if (stateTimer6 >= randomDuration)
        //            {
        //                if (brain.EnemyVision.distance < randomDistance)
        //                {
        //                    hasMovebackThisState = true;
        //                    brain.EnemySteering.RetreatIfCloseTo(brain.EnemyVision.targetDetected);
        //                    if (WoobleInPlace == false)
        //                    {
        //                        WoobleInPlace = true;
        //                        wobbleTimer = 0f;
        //                        brain.EnemySteering.StartWobbleInPlace(duration: randomDuration2, radius: 0.4f, speed: 1.2f);
        //                    }
        //                    else
        //                    {
        //                        wobbleTimer += Time.deltaTime;
        //                        if (wobbleTimer >= smaillDuration)
        //                        {
        //                            brain.ChangeState(new DecisionStage_06(brain, DecisionStage_06.LastActionType.Shot));
        //                        }
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                if (randChoice3 <= 1f / 5f)
        //                {
        //                    brain.EnemySteering.MoveTo(brain.EnemyVision.targetDetected.position, 0.5f);
        //                    if (brain.EnemyVision.distance < 3.2f)
        //                    {
        //                        brain.EnemySteering.StopMoving();
        //                        bulletSwapm.ShotCondition();
        //                        if (IsFinished)
        //                        {
        //                            brain.ChangeState(new movestatetest(brain, new ShotStagetest(brain, null)));
        //                        }
        //                    }
        //                }

        //                else if (randChoice3 <= 2f / 5f)
        //                {
        //                    brain.ChangeState(new DirtCon(brain, new ShotStagetest(brain, null)));
        //                }
        //                else if (randChoice3 <= 3f / 5f)
        //                {
        //                    brain.ChangeState(new movestatetest(brain, new ShotStagetest(brain, null)));
        //                }
        //                else if (randChoice3 <= 4f / 5f)
        //                {
        //                    brain.ChangeState(new DecisionStage_06(brain, DecisionStage_06.LastActionType.Shot));
        //                }
        //                else
        //                {
        //                    brain.EnemySteering.StartWobbleInPlace(duration: randomDuration2, radius: 0.4f, speed: 1.2f);

        //                    stateTimer4 += Time.deltaTime;
        //                    if (stateTimer4 <= randomDuration3)
        //                    {
        //                        rangedEnemyController.ArcAroundPlayer();
        //                    }
        //                    else
        //                    {
        //                        bulletSwapm.ShotCondition();
        //                        brain.ChangeState(new DecisionStage_06(brain, DecisionStage_06.LastActionType.Shot));
        //                    }
        //                }

        //            }
                        
                        
        //        }
        //    }

        //}
                       
    }
    

    public override void Exit()
    {
        bulletSwapm.OnShotBulletFinished -= FinishStage;
    }

    void FinishStage() => IsFinished = true;
}
