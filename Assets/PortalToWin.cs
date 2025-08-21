 using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToWin : MonoBehaviour
{
    [SerializeField] private string winSceneName = "WinScene";

    [Header("Portal Settings")]
    public ThemeManager themeManager;
    public SimpleDungeonGenerator dungeonGenerator;
    public Transform Player;

    private void Awake()
    {
        
    }

    public void Start()
    {
        themeManager = FindFirstObjectByType<ThemeManager>();
        dungeonGenerator = FindFirstObjectByType<SimpleDungeonGenerator>();
        Player = FindFirstObjectByType<PlayerStats>().transform;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && themeManager.stageIndexInTheme < 9)
        {
            Debug.Log("[Portal] Player chạm portal → chuyển sang scene thắng.");
            //SceneManager.LoadScene(winSceneName);
            Player.position = Vector2.zero; // Đặt lại vị trí của Player về 0,0
            dungeonGenerator.ResetDungeon(); // Reset dungeon nếu cần
            themeManager.NextStage(); // Tăng stage trong theme hiện tại
        }
    }


    //Delete all objects in the scene except objects with tag "Player"
    private void OnDestroy()
    {
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.tag == "Items" || obj.tag == "Item")
            {
                Destroy(obj);
            }
        }
    }
}
