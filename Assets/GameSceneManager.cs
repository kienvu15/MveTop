using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
    public Transform spawnPoint;
    public LoadingScreen loadingScreen;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public IEnumerator Summon()
    {
        yield return new WaitForSeconds(2f);
        GameObject prefab = CharacterSelectionData.Instance.selectedCharacterPrefab;
        if (prefab != null)
        {
            GameObject player = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            GameObject rouge = player.GetComponentInChildren<PlayerStats>().gameObject;
            // ✅ Gán player vào Cinemachine
            CinemachineCamera vcam = FindFirstObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                vcam.Follow = rouge.transform;
                vcam.LookAt = rouge.transform;
            }
        }
        else
        {
            Debug.LogError("Không tìm thấy nhân vật để spawn!");
        }
    }
}
