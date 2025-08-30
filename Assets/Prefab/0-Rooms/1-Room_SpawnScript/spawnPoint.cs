using UnityEngine;

public class spawnPoint : MonoBehaviour
{
    public SimpleDungeonGenerator simpleDungeonGenerator;
    public GameObject blocker;
    public bool hasSpawned = false;

    private void Start()
    {
        simpleDungeonGenerator = FindFirstObjectByType<SimpleDungeonGenerator>();
        
    }

    private void Update()
    {
        if (simpleDungeonGenerator != null && simpleDungeonGenerator.isGeneratingDone == true)
        {
            Blocker blockerComp = GetComponentInChildren<Blocker>();

            if (blockerComp != null)
            {
                blocker = blockerComp.gameObject;
                blocker.SetActive(false);
            }
            else
            {
                // Nếu không có blocker thì bỏ qua
                return;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
