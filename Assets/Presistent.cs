using UnityEngine;

public class Presistent : MonoBehaviour
{
    private static Presistent instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // Giữ lại khi load scene mới
        }
        else
        {
            Destroy(gameObject); // Nếu đã có, huỷ bản mới
        }
    }

}
