//using UnityEngine;

//public class PanicRunState : EnemyState
//{
//    private EnemyPanicRun panic;

//    public PanicRunState(EnemyBrain brain) : base(brain) { }

//    public override void Enter()
//    {
//        panic = brain.GetComponent<EnemyPanicRun>();
//        if (panic == null)
//        {
//            Debug.LogError("No EnemyPanicRun component!");
//            brain.ChangeState(new IdleState(brain));
//            return;
//        }

//        panic.StartPanic();
//        panic.OnPanicComplete += OnPanicDone;
//    }


//    private void OnPanicDone()
//    {
//        brain.ChangeState(new IdleState(brain)); // hoặc state khác
//    }

//    public override void Exit()
//    {
//        if (panic != null)
//        {
//            panic.StopPanic();
//            panic.OnPanicComplete -= OnPanicDone;
//        }
//    }
//}