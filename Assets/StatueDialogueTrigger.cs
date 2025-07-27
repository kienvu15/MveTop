using UnityEngine;

public class StatueDialogueTrigger : MonoBehaviour
{
    public GameObject dialogueUI;
    public TypewriterEffect typewriter;
    public string dialogueText = "Một trận Bạch Đằng, muôn thuở lưu danh – nơi giặc đến, ta dựng cọc, dựng cờ, dựng non sông!";

    public PlayerStateController playerStateController; // Gán script có CanMove

    private bool playerInRange = false;
    private bool dialogueActive = false;

    private void Start()
    {
        playerStateController = FindFirstObjectByType<PlayerStateController>();
    }
    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.N))
        {
            if (!dialogueActive)
            {
                dialogueUI.SetActive(true);
                typewriter.StartTyping(dialogueText);
                playerStateController.canMove = false;
                dialogueActive = true;
            }
            else
            {
                if (typewriter.IsTyping)
                {
                    typewriter.Skip(); // hiện hết text ngay
                }
                else
                {
                    dialogueUI.SetActive(false);
                    playerStateController.canMove = true;
                    dialogueActive = false;
                }
            }
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
        {
            playerInRange = false;
            dialogueUI.SetActive(false);
            playerStateController.canMove = true;
            dialogueActive = false;
        }
    }
}
