using System.Collections.Generic;
using UnityEngine;

public class TortureBar_Item : Item
{

    public Transform playerCenter;
    public GameObject GameObject;

    void Start()
    {
        playerCenter = FindFirstObjectByType<PlayerFlip>().transform;
    }

    public override void ApplyEffect(GameObject player)
    {
        Instan();
    }
    public override void RemoveEffect(GameObject player)
    {
        Destroy(FindAnyObjectByType<TortureTrigger>().gameObject);
    }

    public void Instan()
    {
        GameObject newObject = Instantiate(GameObject, playerCenter.position, Quaternion.identity);
        newObject.transform.SetParent(playerCenter); // Set làm con   
    }

}