using UnityEngine;
using System.Collections.Generic;

public class StatueDialogueTrigger : MonoBehaviour
{
    public Statue statue;
    public GameObject dialogueUI;
    public TypewriterEffect typewriter;

    [TextArea] public string dialogueText = "Chạm vào thịnh nộ";
    private string dialogueCostText;

    public PlayerStateController playerStateController;

    private bool playerInRange = false;
    private bool dialogueActive = false;

    private Queue<string> dialogueQueue; // hàng đợi thoại
    private bool isOnCostLine = false;   // flag để biết đang đứng ở dòng cost

    private void Start()
    {
        playerStateController = FindFirstObjectByType<PlayerStateController>();
        statue = GetComponent<Statue>();
        dialogueCostText = $"Nhận phước lành với giá <color=#FFD700>{statue.costCoin}</color> xu.";

        dialogueQueue = new Queue<string>();
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.N))
        {
            if (!dialogueActive)
            {
                // chuẩn bị hàng đợi
                dialogueQueue.Clear();
                dialogueQueue.Enqueue(dialogueText);
                dialogueQueue.Enqueue(dialogueCostText);

                dialogueUI.SetActive(true);
                ShowNextLine();
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
                    // Nếu đang ở dòng cost và player nhấn N lần nữa
                    if (isOnCostLine)
                    {
                        statue.BuyBlessing();

                        if (!statue.canBuy)
                        {
                            dialogueQueue.Enqueue("<color=red>Không đủ tiền!</color>");
                        }

                        isOnCostLine = false; // reset flag
                        ShowNextLine();
                    }
                    else
                    {
                        ShowNextLine();
                    }
                }
            }
        }
    }

    private void ShowNextLine()
    {
        if (dialogueQueue.Count > 0)
        {
            string nextLine = dialogueQueue.Dequeue();
            typewriter.StartTyping(nextLine);

            // Nếu line vừa hiện là dòng cost -> set flag
            if (nextLine.Contains("phước lành với giá"))
            {
                isOnCostLine = true;
            }
            else
            {
                isOnCostLine = false;
            }
        }
        else
        {
            // hết thoại
            dialogueUI.SetActive(false);
            playerStateController.canMove = true;
            dialogueActive = false;
            isOnCostLine = false;
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
            isOnCostLine = false;
        }
    }
}
