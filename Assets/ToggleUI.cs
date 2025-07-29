using UnityEngine;
using UnityEngine.UI;

public class ToggleUI : MonoBehaviour
{
    public Button[] Tap;
    public GameObject LoginuiPanel;


    public void LoginPopUp()
    {
        if (LoginuiPanel != null)
            LoginuiPanel.SetActive(!LoginuiPanel.activeSelf);
        foreach (Button button in Tap)
        {
            button.gameObject.SetActive(!button.gameObject.activeSelf);
        }
    }
}
