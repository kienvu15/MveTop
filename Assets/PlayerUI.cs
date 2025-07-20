using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthSlider;
    public TMP_Text healthText;

    [Header("Ammo UI")]
    public Slider ammoSlider;
    public TMP_Text ammoText;

    [Header("Mana UI")]
    public Slider manaSlider;
    public TMP_Text manaText;

    public void SetHealth(float current, float max)
    {
        healthSlider.maxValue = max;
        healthSlider.value = current;
        healthText.text = $"{current}/{max}";

        // Ẩn Fill Image khi hết máu
        Image fillImage = healthSlider.fillRect.GetComponent<Image>();
        if (current <= 0)
            fillImage.enabled = false;
        else
            fillImage.enabled = true;
    }


    public void SetAmmo(float current, float max)
    {
        ammoSlider.maxValue = max;
        ammoSlider.value = current;
        ammoText.text = $"{current}/{max}";

        Image fillImage = ammoSlider.fillRect.GetComponent<Image>();
        if (current <= 0)
            fillImage.enabled = false;
        else
            fillImage.enabled = true;
    }

    public void SetMana(float current, float max)
    {
        manaSlider.maxValue = max;
        manaSlider.value = current;
        manaText.text = $"{current}/{max}";
    }

}
