//using UnityEngine;

//[CreateAssetMenu(menuName = "Blessing/BlessingBase")]
//public class ScriptableBlessing : ScriptableObject
//{
//    public string blessingName;
//    public string description;

//    public BlessingType type;

//    public void Activate(GameObject player)
//    {
//        // Xử lý tùy theo loại
//        switch (type)
//        {
//            case BlessingType.Heal:
//                //player.GetComponent<PlayerStats>()?.Heal(3);
//                break;

//            case BlessingType.SpeedBoost:
//                //player.GetComponent<PlayerMovement>()?.BoostSpeed(5f, 2f); // VD
//                break;

//            case BlessingType.SummonGuard:
//                // instantiate guardPrefab ở gần player
//                break;
//        }
//    }
//}

//public enum BlessingType
//{
//    Heal,
//    SpeedBoost,
//    SummonGuard,
//    // mở rộng dễ dàng
//}
