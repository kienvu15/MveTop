using UnityEngine;

public class Statue : MonoBehaviour
{
    public enum BlessingType { Heal, Blast }

    public BlessingType statueBlessing;

    public RadialBlastBlessing radialBlastAsset;

    private bool playerInRange = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            IPlayerBlessing blessing = null;

            switch (statueBlessing)
            {
                case BlessingType.Heal:
                    blessing = new HealBlessing(); // vẫn OK nếu không phải ScriptableObject
                    break;

                case BlessingType.Blast:
                    blessing = radialBlastAsset; // ✅ dùng asset đã gán
                    break;
            }


            if (blessing == null)
            {
                Debug.LogWarning("Không tạo được blessing!");
                return;
            }

            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                Debug.LogWarning("Không tìm thấy Player với tag!");
                return;
            }

            var holder = player.GetComponent<PlayerBlessingHolder>();
            if (holder == null)
            {
                Debug.LogWarning("Player không có PlayerBlessingHolder!");
                return;
            }

            holder.AssignBlessing(blessing);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
