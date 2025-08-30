using UnityEngine;

public class StartPointMaker : MonoBehaviour
{
    public EndPointMarker endPointMarker;
    public SimpleDungeonGenerator simpleDungeonGenerator;
    public GameObject blockPrefab;

    public bool isDone = false;
    
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
        if (collision.CompareTag("BlockPoint"))
        {
            if(endPointMarker.isWall == true)
            {
                BlockPoint blockPoint = collision.GetComponent<BlockPoint>();
                if (blockPoint != null)
                {
                    Transform spawnPoint = blockPoint.transform;
                    GameObject obj = Instantiate(blockPrefab, spawnPoint.position, spawnPoint.rotation);
                    obj.transform.SetParent(spawnPoint);
                    isDone = true;
                }
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
        if (collision.CompareTag("BlockPoint"))
        {
            if(endPointMarker.isWall == true)
            {
                BlockPoint blockPoint = collision.GetComponent<BlockPoint>();
                if (blockPoint != null)
                {
                    Transform spawnPoint = blockPoint.transform;
                    GameObject obj = Instantiate(blockPrefab, spawnPoint.position, spawnPoint.rotation);
                    obj.transform.SetParent(spawnPoint);
                    isDone = true;
                }
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
