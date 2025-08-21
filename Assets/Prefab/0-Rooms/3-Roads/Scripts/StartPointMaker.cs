using UnityEngine;

public class StartPointMaker : MonoBehaviour
{
    public EndPointMarker endPointMarker;
    public SimpleDungeonGenerator simpleDungeonGenerator;
    public GameObject blockPrefab;

    public bool isDone = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        simpleDungeonGenerator = FindFirstObjectByType<SimpleDungeonGenerator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DoorPoint"))
        {
            if(endPointMarker.isWall == true)
            {
                Transform spawnPoint = collision.GetComponent<spawnPoint>().transform;
                GameObject obj = Instantiate(blockPrefab, spawnPoint.position, spawnPoint.rotation);
                obj.transform.SetParent(spawnPoint); // Set làm con của spawnPoint
                isDone = true;
            } 
        }

        if (collision.CompareTag("Blocker"))
        {
            if (simpleDungeonGenerator.isGeneratingDone == true)
            {
                GameObject blocker = collision.gameObject.CompareTag("Blocker") ? collision.gameObject : null;
                if (blocker != null)
                {
                    blocker.SetActive(false); // Tắt Blocker khi đã hoàn thành việc tạo đường đi
                    Debug.Log($"🧱 Blocker {blocker.name} đã bị tắt sau khi tạo đường đi.");
                }
            }
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("DoorPoint"))
        {
            if(endPointMarker.isWall == true)
            {
                Transform spawnPoint = collision.GetComponent<spawnPoint>().transform;
                GameObject obj = Instantiate(blockPrefab, spawnPoint.position, spawnPoint.rotation);
                obj.transform.SetParent(spawnPoint); // Set làm con của spawnPoint
                isDone = true;
            }
        }

        if (collision.CompareTag("Blocker"))
        {
            if(simpleDungeonGenerator.isGeneratingDone == true)
            {
                GameObject blocker = collision.gameObject.CompareTag("Blocker") ? collision.gameObject : null;
                if (blocker != null)
                {
                    blocker.SetActive(false); // Tắt Blocker khi đã hoàn thành việc tạo đường đi
                    Debug.Log($"🧱 Blocker {blocker.name} đã bị tắt sau khi tạo đường đi.");
                }
            }
        }
    }

    
}
