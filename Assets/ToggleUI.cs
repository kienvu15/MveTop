using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class ToggleUI : MonoBehaviour
{
    public Button[] Tap;
    public GameObject LoginuiPanel;
    public AudioClip clickSound;

    public void PopUp()
    {
        SFXManager.Instance.PlaySFX(clickSound);
        if (LoginuiPanel != null)
            LoginuiPanel.SetActive(!LoginuiPanel.activeSelf);

        if(Tap == null || Tap.Length == 0) return;
        foreach (Button button in Tap)
        {
            button.gameObject.SetActive(!button.gameObject.activeSelf);
        }
    }
}
