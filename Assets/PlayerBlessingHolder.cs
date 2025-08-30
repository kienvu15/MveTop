using UnityEngine;

public class PlayerBlessingHolder : MonoBehaviour
{
    private IPlayerBlessing currentBlessing;
    public GameObject Feet;

    void Update()
    {
        if (currentBlessing != null && Input.GetKeyDown(currentBlessing.ActivationKey))
        {
            currentBlessing.Activate(gameObject);
        }
    }

    public void AssignBlessing(IPlayerBlessing blessing)
    {
        // Xoá tất cả object con của Feet (nếu có)
        if (Feet != null && Feet.transform.childCount > 0)
        {
            for (int i = Feet.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(Feet.transform.GetChild(i).gameObject);
            }
        }

        // Gán blessing mới
        currentBlessing = blessing;
        Debug.Log("Player nhận được blessing: " + blessing.BlessingName);
    }

}
