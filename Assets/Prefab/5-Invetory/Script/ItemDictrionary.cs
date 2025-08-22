using System.Collections.Generic;
using UnityEngine;

public class ItemDictrionary : MonoBehaviour
{
    public List<Item> itemsPrefab;
    private Dictionary<int, GameObject> itemDictionary;

    public void Awake()
    {
        for (int i = 0; i < itemsPrefab.Count; i++)
        {
            if (itemsPrefab[i] != null)
            {
                itemsPrefab[i].ID = i + 1;
            }
        }

        foreach(Item item in itemsPrefab)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
    }

    public GameObject GetItemPrefab(int itemID)
    {
        itemDictionary.TryGetValue(itemID, out GameObject prefab);
        if (prefab != null) 
        {
            Debug.LogWarning($"Item with ID {itemID} not found");
        }
        return prefab;
    }
}
