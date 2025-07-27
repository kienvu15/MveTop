using UnityEngine;
using TMPro; // Nếu dùng TextMeshPro

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;
    public int coinCount = 0;
    public TMP_Text coinText;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    public void AddCoin(int amount)
    {
        coinCount += amount;
        UpdateCoinUI();
    }

    public void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = coinCount.ToString();
    }
}
