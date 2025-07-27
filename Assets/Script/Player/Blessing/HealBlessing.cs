using UnityEngine;

public class HealBlessing : IPlayerBlessing
{
    public string BlessingName => "Hồi máu";
    

    public void Activate(GameObject player)
    {
        
         Debug.Log("Đã dùng Blessing: Hồi 3 máu");
        
    }
}
