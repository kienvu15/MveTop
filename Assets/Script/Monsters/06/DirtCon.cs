using Unity.VisualScripting;
using UnityEngine;
using static EnemySteering;

public class DirtCon : EnemyState
{
    public DirtCon(EnemyBrain brain, EnemyState nextStage) : base(brain, nextStage)
    {
        this.nextStage = nextStage;
    }

    public RangedEnemyController rangedEnemyController;
    public BulletSwapm bulletSwapm;

    private float stateTimer = 0f;
    private float dirTimer = 0f;
    private float retreatTime = 0f;
    private float wobbleTimer = 0f;
    private float changetime = 0f;
    private float changetime2 = 0f;
    private float lasttim = 0f;

    private bool hasStartedWobble = false;
    private float randomDuration;
    private float randomDuration2;
    private float randomDuration3;

    private float randomSpeed;

    private float randChoice;
    private float randChoice2;
    private float randChoice3;

    private bool hasMovebackThisState = false;
    private bool iswobbleTimer = false;
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Dỉtir State");

        rangedEnemyController = brain.GetComponent<RangedEnemyController>();
        bulletSwapm = brain.GetComponent<BulletSwapm>();
        rangedEnemyController.OnDritFinished += FinishStage;
        bulletSwapm.OnShotBulletFinished += FinishStage;

        hasMovebackThisState = false;

        randChoice3 = Random.value;
        randChoice2 = Random.value;
        randChoice = Random.value;

        randomDuration = Random.Range(0.5f, 1.1f);
        randomDuration3 = Random.Range(1f, 3.2f);
        randomDuration2 = Random.Range(0.7f, 1.9f);
        randomSpeed = Random.Range(1f, 1.8f);

        stateTimer = 0f;
        wobbleTimer = 0f;
        dirTimer = 0f;
        retreatTime = 0f;
        changetime = 0f;
        changetime2 = 0f;
        lasttim = 0f;

        hasStartedWobble = false;
        iswobbleTimer = false;
    }

    public override void Update()
    {
        

        if(randChoice < 1f / 3f)
        {
            if (brain.EnemyVision.distance < 3.3f)
            {
                if (!hasMovebackThisState)
                {
                    Debug.Log("Quá gần, lùi lại");
                    hasMovebackThisState = true;
                    //rangedEnemyController.RetreatCondition();
                }
                else
                {
                    Debug.Log("dợi chuyển stage");
                    retreatTime += Time.deltaTime;
                    if (retreatTime >= randomDuration)
                    {
                       // rangedEnemyController.StopReytread();
                        //brain.ChangeState(new ShotStagetest(brain, new movestatetest(brain, null)));
                    }
                }
            }
            else if (brain.EnemyVision.distance > 3.3f)
            {
                Debug.Log("Khoảng cách oke bắt đầu Wobb"); 
                if (hasStartedWobble == false)
                {
                    hasStartedWobble = true;
                    wobbleTimer = 0f;
                    rangedEnemyController.DritDec();
                   // brain.EnemySteering.StartWobbleInPlace(duration: randomDuration2, radius: 1f, speed: 1.2f);
                }
                else
                {
                    wobbleTimer += Time.deltaTime;
                    if (wobbleTimer >= randomDuration2)
                    {
                        Debug.Log("Wobb kết thúc chuyển state");
                       // /brain.ChangeState(new ShotStagetest(brain, new movestatetest(brain, null)));
                    }
                }
            }
        }

        ////////
        
        else if(randChoice < 2f / 3f)
        {
            if (brain.EnemyVision.distance > 3.5f && brain.EnemyVision.CanSeePlayer == true)
            {
                Debug.Log("Player is too far, switching to movement state");
                

                if (!brain.EnemySteering.hasChosenCurve)
                {
                    brain.EnemySteering.chosenCurveMode = (CurveMode)Random.Range(0, 3);
                    brain.EnemySteering.hasChosenCurve = true;

                    if (brain.EnemySteering.chosenCurveMode == CurveMode.LoopBack)
                        brain.EnemySteering.SetRandomLoopbackOffset();
                }

                brain.EnemySteering.MoveToWithBendSmart(brain.EnemyVision.targetDetected.position, brain.EnemySteering.chosenCurveMode, randomSpeed);

            }
            
            else if (brain.EnemyVision.distance < 2.4f)
            {
                Debug.Log("Di chuyển đủ gần");
                if (randChoice2 > 0.5f)
                {
                    rangedEnemyController.DritDec();
                    Debug.Log("Random dre");
                    stateTimer += Time.deltaTime;
                    if (stateTimer >= randomDuration)
                    {
                        rangedEnemyController.StopDritDec();
                        bulletSwapm.ShotCondition();
                        Debug.Log("⏱ Timer kết thúc trong DirtCon.");
                        
                        brain.ChangeState(new ShotStagetest(brain, new DirtCon(brain, null)));
                    }
                }

                ///////

                else
                {
                    Debug.Log("Dir bất chấp");
                    dirTimer += Time.deltaTime;
                    if (dirTimer <= randomDuration)
                    {
                        rangedEnemyController.DritDec();
                    }
                    else
                    {
                        Debug.Log("Hết Time dừng dir");
                        rangedEnemyController.StopDritDec();
                        if (randChoice3 > 0.5f)
                        {
                            if(iswobbleTimer == false)
                            {
                                iswobbleTimer = true;
                                wobbleTimer = 0f;
                                brain.EnemySteering.StartWobbleInPlace(duration: randomDuration, radius: 0.4f, speed: 1f);
                                Debug.Log("Uoowoob");
                            }
                            else
                            {
                                wobbleTimer += Time.deltaTime;
                                if (changetime <= randomDuration2)
                                {
                                    Debug.Log("Woob xong đổi state");
                                    brain.ChangeState(new ShotStagetest(brain, new DirtCon(brain, null)));

                                }
                            }
                        }

                        ////
                        
                        else
                        {
                            changetime2 += Time.deltaTime;
                            if (changetime2 <= randomDuration3)
                            {
                                Debug.Log("Dire 50%");
                                rangedEnemyController.DritDec();
                            }
                            else
                            {
                                Debug.Log("Dỉe 505 xong đổi state");
                                rangedEnemyController.StopDritDec();
                                brain.ChangeState(new ShotStagetest(brain, new DirtCon(brain, null)));
                            }
                        }
                    }
                }

            }
        }


        else
        {
            if (hasStartedWobble == false)
            {
                hasStartedWobble = true;
                wobbleTimer = 0f;
                brain.EnemySteering.StartWobbleInPlace(duration: randomDuration2, radius: 0.4f, speed: 1.2f);
            }
            else
            {
                wobbleTimer += Time.deltaTime;
                if (wobbleTimer <= randomDuration)
                {
                    if (randChoice3 > 0.5f)
                    {
                        bulletSwapm.ShotCondition();
                        if (bulletSwapm.isShooting == false)
                        {
                          // brain.ChangeState(new movestatetest(brain, new DecisionStage_06(brain)));
                        }
                    }
                    else
                    {
                        lasttim += Time.deltaTime;
                        if (lasttim <= randomDuration)
                        {
                            rangedEnemyController.DritDec();
                        }
                        else
                        {
                            rangedEnemyController.StopDritDec();
                           // brain.ChangeState(new ShotStagetest(brain, new movestatetest(brain, null)));
                        }
                    }
                }
            }
        }
        


    }

    public override void Exit()
    {
        
    }
    void FinishStage() => IsFinished = true;
}
