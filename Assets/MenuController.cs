using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public GameObject menuCanvas;
    public GameObject PlayerStatsUI;

    public Button Pause;
    public Button Resume;

    public AudioClip clickSound;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        menuCanvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            SFXManager.Instance.PlaySFX(clickSound);

            menuCanvas.SetActive(!menuCanvas.activeSelf);
            PlayerStatsUI.SetActive(!PlayerStatsUI.activeSelf);
            Pause.gameObject.SetActive(!Pause.gameObject.activeSelf);
            Resume.gameObject.SetActive(!Resume.gameObject.activeSelf);
        }
    }

    public void ToggleMenu()
    {
        SFXManager.Instance.PlaySFX(clickSound);

        menuCanvas.SetActive(!menuCanvas.activeSelf);
        PlayerStatsUI.SetActive(!PlayerStatsUI.activeSelf);
        Pause.gameObject.SetActive(!Pause.gameObject.activeSelf);
        Resume.gameObject.SetActive(!Resume.gameObject.activeSelf);
    }
}
