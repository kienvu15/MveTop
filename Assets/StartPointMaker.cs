using UnityEngine;

public class StartPointMaker : MonoBehaviour
{
    public EndPointMarker endPointMarker;
    public GameObject blockPrefab;

    public bool isDone = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
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
    }

    
}
