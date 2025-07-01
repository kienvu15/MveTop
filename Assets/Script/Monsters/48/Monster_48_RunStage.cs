using UnityEngine;

public class Monster_48_RunStage : EnemyState
{
    public EnemyBounceRun EnemyBounceRun;
    public Monster_48_RunStage(EnemyBrain brain, EnemyState nextStage) : base(brain)
    {
        this.nextStage = nextStage;
    }

    public override void Enter()
    {
        base.Enter();
        EnemyBounceRun = brain.GetComponent<EnemyBounceRun>();
        EnemyBounceRun.OnRunFinished += FinishStage;
    }
     
    // Update is called once per frame
    public override void Update()
    {
        EnemyBounceRun.HandelConditionRun();
        if (IsFinished)
            brain.ChangeState(nextStage);
    }
    
    public override void Exit()
    {
        EnemyBounceRun.OnRunFinished -= FinishStage;
    }

    void FinishStage() => IsFinished = true;
}
