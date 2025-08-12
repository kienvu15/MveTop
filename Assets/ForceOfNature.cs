using UnityEngine;

public class ForceOfNature : MonoBehaviour
{
    public PlayerStateController playerStateController;
    public GameObject Force;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateController = FindFirstObjectByType<PlayerStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playerStateController.hurt == true)
        {
            Force.SetActive(true);
        }
    }


}
