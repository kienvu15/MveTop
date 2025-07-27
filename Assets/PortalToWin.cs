using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalToWin : MonoBehaviour
{
    [SerializeField] private string winSceneName = "WinScene";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[Portal] Player chạm portal → chuyển sang scene thắng.");
            SceneManager.LoadScene(winSceneName);
        }
    }
}
