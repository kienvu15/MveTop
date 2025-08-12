using UnityEngine;

public class DeathWater_Item : Item
{
    public Transform playerCenter;
    public PlayerStateController playerStateController;
    public GameObject ToxicGas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCenter = FindFirstObjectByType<PlayerFlip>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void ApplyEffect(GameObject player)
    {
        Instan();
    }
    public override void RemoveEffect(GameObject player)
    {
        Destroy(FindAnyObjectByType<ToxicGas>().gameObject);
    }

    public void Instan()
    {
        GameObject newObject = Instantiate(ToxicGas, playerCenter.position, Quaternion.identity);
        newObject.transform.SetParent(playerCenter); // Set làm con   
    }
}
