using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;
    public bool isSpecialItem;
    public bool isEquipped = false;


    public virtual void Pickup()
    {
        Sprite itemIcon = GetComponent<Image>().sprite;
        if(ItemPickUpUIController.Instance != null)
        {
            ItemPickUpUIController.Instance.ShowItemPopup(Name, itemIcon);
        }
    }

    public virtual void ApplyEffect(GameObject player)
    {
        Debug.Log($"[+] {Name} effect applied to {player.name}");
        // Example: player.GetComponent<PlayerStats>().attack += 5;
    }

    public virtual void RemoveEffect(GameObject player)
    {
        Debug.Log($"[-] {Name} effect removed from {player.name}");
        // Example: player.GetComponent<PlayerStats>().attack -= 5;
    }



}
