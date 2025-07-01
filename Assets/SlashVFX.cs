using UnityEngine;

public class SlashVFX : MonoBehaviour
{
    public Transform playerSprite;

    void Start()
    {
        gameObject.SetActive(false); // VFX ban đầu không hiển thị
    }

    void Update()
    {
        
    }

    void Disable()
    {
        gameObject.SetActive(false); // pooling-friendly
    }
}
