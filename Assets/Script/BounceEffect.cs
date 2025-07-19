using System.Collections;
using UnityEngine;

public class BounceEffect : MonoBehaviour
{
    public float bounceHeight = 0.4f;
    public float bounceDuration = 0.4f;
    public int bounceCount = 2;

    public void StartBounce()
    {
        StartCoroutine(BounceHanlder());
    }

    private IEnumerator BounceHanlder()
    {
        Vector3 startPosition = transform.position;
        float localHieght = bounceHeight;
        float localDuration = bounceDuration;

        for (int i = 0; i < bounceCount; i++)
        {
            yield return Bounce(transform, startPosition, localHieght, localDuration / 2);
            localHieght *= 0.5f;
            localDuration *= 0.8f;
        }

        transform.position = startPosition;
    }

    private IEnumerator Bounce(Transform objectTransform, Vector3 start, float hieght, float duration)
    {
        Vector3 peak = start + Vector3.up * hieght;
        float elapsed = 0f;

        while (elapsed < duration) 
        { 
            objectTransform.position = Vector3.Lerp(start, peak, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < duration)
        {
            objectTransform.position = Vector3.Lerp(peak, start, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }
}
