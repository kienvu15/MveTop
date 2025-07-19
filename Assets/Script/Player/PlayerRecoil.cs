using System.Collections;
using UnityEngine;

public class PlayerRecoil : MonoBehaviour
{
 
    private Rigidbody2D rb;
    private PlayerStateController stateController;
    private PlayerStats PlayerStats;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        stateController = GetComponent<PlayerStateController>();
        PlayerStats = GetComponent<PlayerStats>();
    }

    public void ApplyRecoil(Vector2 attackPointPosition)
    {
        Vector2 recoilDir = ((Vector2)transform.position - attackPointPosition).normalized;
        rb.AddForce(recoilDir * PlayerStats.recoilForce, ForceMode2D.Impulse);
        StartCoroutine(RecoilRoutine());
    }

    private IEnumerator RecoilRoutine()
    {
        stateController.isRecoiling = true;
        yield return new WaitForSeconds(0.15f);
        stateController.isRecoiling = false;
    }
}
