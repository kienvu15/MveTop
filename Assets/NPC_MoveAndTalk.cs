using UnityEngine;

public class NPC_MoveAndTalk : MonoBehaviour
{
    public Transform playerStopPoint;   // Điểm dừng trước Player
    public Transform exitPoint;         // Điểm chạy đi sau khi nói xong
    public float moveSpeed = 2f;

    private enum State { MovingToPlayer, Talking, MovingAway }
    private State currentState = State.MovingToPlayer;

    private DialoguePopup dialoguePopup;

    void Start()
    {
        dialoguePopup = FindFirstObjectByType<DialoguePopup>();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.MovingToPlayer:
                MoveTo(playerStopPoint.position, () =>
                {
                    currentState = State.Talking;
                    dialoguePopup.ShowDialogue("Công tử! Phía trước là quân Nguyên! Mau chuẩn bị!");
                });
                break;

            case State.Talking:
                if (!dialoguePopup.isShowing)
                {
                    currentState = State.MovingAway;
                }
                break;

            case State.MovingAway:
                MoveTo(exitPoint.position, () =>
                {
                    Destroy(gameObject);
                });
                break;
        }
    }

    private void MoveTo(Vector3 target, System.Action onArrive)
    {
        transform.position = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (Vector2.Distance(transform.position, target) < 0.05f)
        {
            onArrive?.Invoke();
        }
    }
}
