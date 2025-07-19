using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private PlayerStateController playerStateController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerStateController = FindFirstObjectByType<PlayerStateController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = playerStateController.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            StartCoroutine(LoadScene("ss02"));
        }
    }

    public IEnumerator LoadScene(string sceneName)
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(sceneName);  
    }
}
