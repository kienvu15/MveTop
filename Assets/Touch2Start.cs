using UnityEngine;
using UnityEngine.SceneManagement;

public class Touch2Start : MonoBehaviour
{
    public string sceneName = "1-Home";
    public AudioClip clickSound;

    public void LoadScene()
    {
        SFXManager.Instance.PlaySFX(clickSound);
        StartCoroutine(LoadAfterSound());
    }

    private System.Collections.IEnumerator LoadAfterSound()
    {
        float clipLength = clickSound.length;
        yield return new WaitForSeconds(clipLength);
        SceneManager.LoadScene(sceneName);
    }

}
