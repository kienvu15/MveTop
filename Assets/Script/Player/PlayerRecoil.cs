using System.Collections;
using UnityEngine;

public class PlayerRecoil : MonoBehaviour
{
 
    private Rigidbody2D rb;
    private PlayerStateController stateController;
    private PlayerStats Playerstats;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateController = GetComponent<PlayerStateController>();
        Playerstats = GetComponent<PlayerStats>();
    }

    public void ApplyRecoil(Vector2 attackPointPosition)
    {
        Vector2 recoilDir = ((Vector2)transform.position - attackPointPosition).normalized;
        rb.AddForce(recoilDir * Playerstats.recoilForce, ForceMode2D.Impulse);
        StartCoroutine(RecoilRoutine());
    }

    public void ApplyHitedRecoil(Vector2 attackPointPosition)
    {
        Vector2 recoilDir = ((Vector2)transform.position - attackPointPosition).normalized;
        rb.AddForce(recoilDir * Playerstats.recoilHitForce, ForceMode2D.Impulse);
        StartCoroutine(RecoilRoutine());
    }

    private IEnumerator RecoilRoutine()
    {
        stateController.isRecoiling = true;
        yield return new WaitForSeconds(0.15f);
        stateController.isRecoiling = false;
    }
}
