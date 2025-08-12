using UnityEngine;

public class NatureForce : Item
{

    public Transform playerStats;
    public PlayerStateController playerStateController;
    public GameObject Force;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStats = FindFirstObjectByType<PlayerFlip>().transform;
        playerStateController = FindFirstObjectByType<PlayerStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect(GameObject player)
    {
        ForceOfNature();
    }
    public override void RemoveEffect(GameObject player)
    {
        Destroy(FindAnyObjectByType<ForceOfNature>().gameObject);
    }

    public void ForceOfNature()
    {    
            GameObject newObject = Instantiate(Force, playerStats.position, Quaternion.identity);
            newObject.transform.SetParent(playerStats); // Set làm con   
    }
    
}
