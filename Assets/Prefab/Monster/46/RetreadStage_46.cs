using UnityEngine;

public class RetreadStage_46 : EnemyState
{
    public RetreadStage_46(EnemyBrain brain) : base(brain) { }

    public EnemyBounceRun enemyBounceRun;
    public float lastRetreadTime = -Mathf.Infinity;
    public float cooldownAfterRetread = 3f; // 3 giây hồi sau khi retreat
    public override void Enter()
    {
        base.Enter();
        Debug.Log("RetreadStage_46: Entering retread stage");
        enemyBounceRun = brain.GetComponent<EnemyBounceRun>();
        enemyBounceRun.OnRunFinished += FinishStage;
    }

    public override void Update()
    {
        enemyBounceRun.HandelConditionRun();
        if (IsFinished)
        {
            brain.ChangeState(new ArcAround_46(brain));
        }
            
    }

    public override void Exit()
    {
        enemyBounceRun.OnRunFinished -= FinishStage;
    }

    void FinishStage() => IsFinished = true;

}
