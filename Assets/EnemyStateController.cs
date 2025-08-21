  using UnityEngine;

public class EnemyStateController : MonoBehaviour
{
    public bool isRecoiling = false;
    public bool canMove = true;
    public bool canAttack = true;

    public void Update()
    {
        if(isRecoiling == true)
        {
            canMove = false;
        }
        else
        {
            canMove = true;
        }
    }
}
