 using UnityEngine;

public class SlashVFX : MonoBehaviour
{
    void Disable()
    {
        gameObject.SetActive(false); // pooling-friendly
    }
}
