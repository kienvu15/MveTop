//using UnityEngine;

//public class DecisionState : EnemyState
//{
//    public DecisionState(EnemyBrain brain) : base(brain) { }

//    public override void Enter()
//    {
//        // Chọn ngẫu nhiên giữa Charge và PanicRun
//        int choice = Random.Range(0, 2);

//        if (choice == 0)
//        {
//            brain.ChangeState(new ChargeState(brain));
//        }
//        else
//        {
//            brain.ChangeState(new PanicRunState(brain)); // chạy 3 lần
//        }
//    }
//}
