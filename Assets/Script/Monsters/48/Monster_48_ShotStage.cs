using UnityEngine;

public class Monster_48_ShotStage : EnemyState
{
    public Monster_48_ShotStage(EnemyBrain brain, EnemyState nextStage) : base(brain)
    {
        this.nextStage = nextStage;
    }

    public EnemyShot EnemyShot;

    public override void Enter()
    {
        base.Enter();
        EnemyShot = brain.GetComponent<EnemyShot>();
        EnemyShot.OnShotFinished += FinishStage;
    }
    
    public override void Update()
    {
        EnemyShot.ConditionShot();
        if (IsFinished)
            brain.ChangeState(nextStage);
    }
    
    public override void Exit() 
    { 
        EnemyShot.OnShotFinished -= FinishStage;
    }
    void FinishStage() => IsFinished = true;
}
