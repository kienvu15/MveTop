using UnityEngine;
using System.Collections.Generic;

public class VFXPool : MonoBehaviour
{
    public GameObject vfxPrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Start()
    {
        
            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = Instantiate(vfxPrefab, transform);
                obj.SetActive(false);
                pool.Enqueue(obj);
            }
        

    }

    public void PlayVFX(Vector3 position, Quaternion rotation)
    {
        if (pool.Count == 0)
        {
            Debug.LogWarning("Pool empty! Consider increasing poolSize.");
            return;
        }
        GameObject obj = pool.Dequeue();
        obj.transform.SetPositionAndRotation(position, rotation);
        obj.SetActive(true);
        pool.Enqueue(obj);
    }

}
