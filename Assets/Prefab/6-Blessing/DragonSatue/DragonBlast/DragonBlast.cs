using UnityEngine;

[CreateAssetMenu(menuName = "Blessing/DragonBlast")]
public class DragonBlast : ScriptableObject, IPlayerBlessing
{
    public string BlessingName => "DragonBlast";
    public KeyCode ActivationKey => KeyCode.Space; // <- nút kích hoạt riêng

    public GameObject blastPrefab;
    public float blastSpeed = 10f;

    public void Activate(GameObject player)
    {


        if (blastPrefab == null)
        {
            Debug.LogError("blastPrefab chưa được gán!");
            return;
        }

        GameObject blast = GameObject.Instantiate(blastPrefab, player.transform.position, Quaternion.identity);

        Debug.Log("Kích hoạt Radial Blast!");
    }
}
