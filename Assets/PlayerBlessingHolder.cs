using UnityEngine;

public class PlayerBlessingHolder : MonoBehaviour
{
    private IPlayerBlessing currentBlessing;
    public GameObject UI;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && currentBlessing != null)
        {
            currentBlessing.Activate(gameObject);
            
        }
    }

    public void AssignBlessing(IPlayerBlessing blessing)
    {
        currentBlessing = blessing;
        Debug.Log("Player nhận được blessing: " + blessing.BlessingName);
        UI.SetActive(true);
    }
}
