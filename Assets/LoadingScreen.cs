using System.Collections;
using UnityEngine;

public class LoadingScreen : MonoBehaviour
{
    public GameObject UI;
    public GameObject Audio;
    public SimpleDungeonGenerator simpleDungeonGenerator;

    public SpriteRenderer spriteRenderer;
    public GameObject loading;

    private bool hasSpawned = false; // ✅ flag đảm bảo chỉ spawn 1 lần

    void Start()
    {
        simpleDungeonGenerator = FindFirstObjectByType<SimpleDungeonGenerator>();
    }

    void Update()
    {
        if (simpleDungeonGenerator.isGeneratingDone && !hasSpawned)
        {
            StartCoroutine(OpenShow());
            hasSpawned = true; // ✅ tránh gọi nhiều lần
        }
    }

    public IEnumerator OpenShow()
    {
        yield return new WaitForSeconds(2);
        Show();
    }

    public void Show()
    {
        UI.SetActive(true);
        Audio.SetActive(true);
        loading.SetActive(false);
        spriteRenderer.enabled = false;

        // ✅ Gọi spawn thẳng tại đây
        GameSceneManager gsm = FindFirstObjectByType<GameSceneManager>();
        if (gsm != null)
        {
            StartCoroutine(gsm.Summon());
        }
        else
        {
            Debug.LogError("Không tìm thấy GameSceneManager!");
        }
    }
}
