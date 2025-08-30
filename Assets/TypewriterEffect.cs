using System.Collections;
using TMPro;
using UnityEngine;

public class TypewriterEffect : MonoBehaviour
{
    public float delay = 0.05f;
    public TMP_Text dialogueText;

    private Coroutine typeCoroutine;
    private string fullText;
    private bool isTyping;

    public bool IsTyping => isTyping;

    public void StartTyping(string message)
    {
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }

        fullText = message;
        typeCoroutine = StartCoroutine(TypeText(message));
    }

    private IEnumerator TypeText(string message)
    {
        isTyping = true;
        dialogueText.text = "";

        int i = 0;
        while (i < message.Length)
        {
            // Nếu gặp tag mở <...>
            if (message[i] == '<')
            {
                int closingIndex = message.IndexOf('>', i);
                if (closingIndex != -1)
                {
                    string tag = message.Substring(i, closingIndex - i + 1);
                    dialogueText.text += tag;   // Thêm nguyên tag ngay lập tức
                    i = closingIndex + 1;       // Nhảy qua tag
                    continue;
                }
            }

            // Nếu là ký tự thường -> type bình thường
            dialogueText.text += message[i];
            i++;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;
    }

    public void Skip()
    {
        if (isTyping)
        {
            StopCoroutine(typeCoroutine);
            dialogueText.text = fullText;  // Hiện full luôn (cả rich text)
            isTyping = false;
        }
    }
}
