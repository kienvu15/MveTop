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
        foreach (char c in message)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(delay);
        }
        isTyping = false;
    }

    public void Skip()
    {
        if (isTyping)
        {
            StopCoroutine(typeCoroutine);
            dialogueText.text = fullText;
            isTyping = false;
        }
    }
}
