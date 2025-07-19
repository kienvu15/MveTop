using UnityEngine;
using UnityEngine.UI;

public class DialoguePopup : MonoBehaviour
{
    public GameObject dialoguePanel;
    public Text dialogueText;

    public bool isShowing { get; private set; } = false;

    public void ShowDialogue(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = message;
        isShowing = true;
    }

    void Update()
    {
        if (isShowing && Input.GetKeyDown(KeyCode.E))  // Bấm E để đóng
        {
            CloseDialogue();
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        isShowing = false;
    }
}
