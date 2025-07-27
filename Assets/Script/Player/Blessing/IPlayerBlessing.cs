using UnityEngine;

public interface IPlayerBlessing
{
    string BlessingName { get; }
    void Activate(GameObject player);
}
