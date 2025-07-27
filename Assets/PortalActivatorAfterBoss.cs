using UnityEngine;

public class PortalActivatorAfterBoss : MonoBehaviour
{
    [Header("Portal sẽ được kích hoạt khi boss chết")]
    [SerializeField] private GameObject portalToWin;

    private void Awake()
    {
        if (portalToWin != null)
            portalToWin.SetActive(false);  // Ẩn portal ban đầu
    }

    public void OnBossDefeated()
    {
        Debug.Log("[PortalActivator] Boss đã bị tiêu diệt → KÍCH HOẠT PORTAL!");

        if (portalToWin != null)
            portalToWin.SetActive(true);
        else
            Debug.LogWarning("[PortalActivator] Chưa gán portalToWin trong Inspector!");
    }
}
