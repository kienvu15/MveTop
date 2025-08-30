using UnityEngine;

[CreateAssetMenu(menuName = "Blessing/Protect Force")]
public class ProtectForceBlessing : ScriptableObject, IPlayerBlessing
{
    public string BlessingName => "Bảo vệ của nữ thần";
    public KeyCode ActivationKey => KeyCode.Space; // <- nút kích hoạt riêng

    public GameObject blastPrefab;
    //public float blastSpeed = 5f;

    public void Activate(GameObject player)
    {
        if (blastPrefab == null)
        {
            Debug.LogError("blastPrefab chưa được gán!");
            return;
        }

        
        GameObject blast = GameObject.Instantiate(blastPrefab, player.transform.position, Quaternion.identity);
        blast.transform.SetParent(player.transform); // Gắn blast vào player




        Debug.Log("Kích hoạt Radial Blast!");
    }
}
