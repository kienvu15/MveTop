using UnityEngine;

public class PlayerVision : MonoBehaviour
{
    [Range(1f, 20f)]
    public float influenceRadius = 5f;

    public float InfluenceRadius => influenceRadius; // Cho phép truy cập từ LOSManager

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, influenceRadius);
    }
}
