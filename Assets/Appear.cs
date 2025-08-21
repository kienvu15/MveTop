using UnityEngine;

public class Appear : MonoBehaviour
{
    public GameObject targetObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        targetObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        targetObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
