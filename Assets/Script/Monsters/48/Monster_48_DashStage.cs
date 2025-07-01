using UnityEngine;

public class Monster_48_DashStage : EnemyState
{
    public Monster_48_DashStage(EnemyBrain brain) : base(brain) {}

    public EnemyDashAttack EnemyDashAttack;
    public override void Enter()
    {
        base.Enter();
        EnemyDashAttack = brain.GetComponent<EnemyDashAttack>();
        EnemyDashAttack.OnDashFinished += FinishStage;
    }

    // Update is called once per frame
    public override void Update()
    {
        EnemyDashAttack.ConditionDash();
    }
    void FinishStage()
    {
        IsFinished = true;
    }
    public override void Exit() 
    { 
        EnemyDashAttack.OnDashFinished -= FinishStage;
    }
}
