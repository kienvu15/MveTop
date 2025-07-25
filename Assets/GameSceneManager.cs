using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public Transform spawnPoint;

    void Start()
    {
        GameObject prefab = CharacterSelectionData.Instance.selectedCharacterPrefab;
        if (prefab != null)
        {
            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Không tìm thấy nhân vật để spawn!");
        }
    }
}
