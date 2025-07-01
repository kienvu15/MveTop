// EnemyState.cs
public abstract class EnemyState
{
    protected EnemyBrain brain;
    protected EnemyState nextStage;
    public bool IsFinished { get; protected set; }
    public EnemyState(EnemyBrain brain, EnemyState nextStage = null)
    {
        this.brain = brain;
        this.nextStage = nextStage;
    }

    public virtual void Enter() => IsFinished = false;
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    
}
