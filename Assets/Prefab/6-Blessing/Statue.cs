using TMPro;
using UnityEngine;

public class Statue : MonoBehaviour
{
    public enum BlessingType { Heal, Blast, Dragon, Protect }

    public BlessingType statueBlessing;

    public RadialBlastBlessing radialBlastAsset;
    public DragonBlast dragonBlastAsset;
    public ProtectForceBlessing protectForceAsset;
    public bool canBuy = true;
    public int costCoin = 5;

    private bool playerInRange = false;

    public Transform playerFeet;
    public GameObject dragonTatto;
    void Start()
    {
        playerFeet = GameObject.FindWithTag("FeetPlayer").transform;
    }
    void Update()
    {
        

       // SetInfo();
    }

    public void SetInfo()
    {
        //priceText.text = costCoin + "";
    }

    public void BuyBlessing()
    {
        int coin = costCoin;
        if (CoinManager.Instance.coinCount >= costCoin)
        {
            CoinManager.Instance.coinCount -= costCoin;
            CoinManager.Instance.UpdateCoinUI();
            reciveBlessing();
        }
        else
        {
            canBuy = false;
            Debug.Log("Không đủ xu để mua.");
        }
    }

    public void reciveBlessing()
    {
        IPlayerBlessing blessing = null;
        GameObject effectPrefab = null;

        switch (statueBlessing)
        {
            case BlessingType.Heal:
                blessing = new HealBlessing();
                break;

            case BlessingType.Blast:
                blessing = radialBlastAsset;
                effectPrefab = dragonTatto;
                break;

            case BlessingType.Dragon:
                blessing = dragonBlastAsset;
                effectPrefab = dragonTatto;
                break;

            case BlessingType.Protect:
                blessing = protectForceAsset;
                effectPrefab = dragonTatto;
                break;
        }

        if (blessing == null) return;

        var player = GameObject.FindWithTag("Player");
        var holder = player?.GetComponent<PlayerBlessingHolder>();
        if (holder == null) return;

        holder.AssignBlessing(blessing);

        // Spawn effect (sau khi xoá cái cũ nhờ AssignBlessing)
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, playerFeet.position, Quaternion.identity);
            effect.transform.SetParent(playerFeet);
        }
    }



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
        canBuy = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }
}
