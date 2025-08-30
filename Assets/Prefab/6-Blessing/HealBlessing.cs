using UnityEngine;

public class HealBlessing : IPlayerBlessing
{
    public string BlessingName => "Hồi máu";
    public KeyCode ActivationKey => KeyCode.Z; // <- nút kích hoạt riêng

    public void Activate(GameObject player)
    {
        
         Debug.Log("Đã dùng Blessing: Hồi 3 máu");
        
    }
}
