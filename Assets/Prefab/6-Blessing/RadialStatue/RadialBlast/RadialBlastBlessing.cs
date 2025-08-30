using UnityEngine;

[CreateAssetMenu(menuName = "Blessing/Radial Blast")]
public class RadialBlastBlessing : ScriptableObject, IPlayerBlessing
{
    public string BlessingName => "Lửa";
    public KeyCode ActivationKey => KeyCode.Space; // <- nút kích hoạt riêng

    public GameObject blastPrefab;
    public float blastSpeed = 5f;

    public void Activate(GameObject player)
    {
        if (blastPrefab == null)
        {
            Debug.LogError("blastPrefab chưa được gán!");
            return;
        }

        Vector2[] directions = new Vector2[]
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right
        };

        foreach (Vector2 dir in directions)
        {
            GameObject blast = GameObject.Instantiate(blastPrefab, player.transform.position, Quaternion.identity);

            float angleDeg = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            blast.transform.rotation = Quaternion.Euler(0, 0, angleDeg);

            Rigidbody2D rb = blast.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = dir * blastSpeed;
            }
        }

        Debug.Log("Kích hoạt Radial Blast!");
    }
}
