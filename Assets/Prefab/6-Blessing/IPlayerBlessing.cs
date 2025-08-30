using UnityEngine;

public interface IPlayerBlessing
{
    string BlessingName { get; }
    KeyCode ActivationKey { get; }
    void Activate(GameObject player);
}
