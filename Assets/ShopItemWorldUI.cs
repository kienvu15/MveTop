using TMPro;
using UnityEngine;

public class ShopItemWorldUI : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text priceText;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false; // Ẩn ban đầu
        nameText.enabled = false;
        priceText.enabled = false;
    }

    public void SetInfo(string itemName, int price)
    {
        nameText.text = itemName;
        priceText.text = "Giá: " + price + " xu";
    }

    public void ShowWithoutText()
    {
        canvas.enabled = true;
        nameText.enabled = true;
        priceText.enabled = false;
    }

    public void ShowWithText()
    {
        canvas.enabled = true;
        nameText.enabled = true;
        priceText.enabled = true;
    }

    public void Show() => canvas.enabled = true;
    public void Hide() => canvas.enabled = false;
}
