using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickUpUIController : MonoBehaviour
{
    public static ItemPickUpUIController Instance { get; private set; }
    public GameObject popupPrefab;
    public int maxPopups = 5;
    public float popupDuration = 3f;

    private readonly Queue<GameObject> activePopups = new();
    

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowItemPopup(string itemName, Sprite itemIcon)
    {
        GameObject newPopup = Instantiate(popupPrefab, transform);
        newPopup.GetComponentInChildren<TMP_Text>().text = itemName;

        Image itemImage = newPopup.transform.Find("ItemIcon")?.GetComponent<Image>();
        if (itemImage)
        {
            itemImage.sprite = itemIcon;
        }

        activePopups.Enqueue(newPopup);
        if (activePopups.Count > maxPopups)
        {
            Destroy(activePopups.Dequeue());
        }

        StartCoroutine(FadeOutAndDestroy(newPopup));
    }

    private IEnumerator FadeOutAndDestroy(GameObject popup)
    {

        yield return new WaitForSeconds(popupDuration);
        if(popup == null) yield break;
        CanvasGroup canvasGroup = popup.GetComponent<CanvasGroup>();
        for(float timePassed = 0f; timePassed < 1f; timePassed += Time.deltaTime)
        {
            if (canvasGroup == null) yield break;
            canvasGroup.alpha = 1f - timePassed;
            yield return null;
        }

        Destroy(popup);
    }
}
