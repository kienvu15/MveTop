using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectionUI : MonoBehaviour
{
    [Header("References")]
    public List<CharacterData> characters; // Gán trong Inspector
    public GameObject avatarButtonPrefab;  // Prefab có Button + Image
    public Transform gridParent;           // Nơi chứa các nút nhân vật
    public Transform displayParent;        // Nơi hiển thị nhân vật demo
    public Button playButton;              // Nút "Chơi"
    public TMP_Text nameText;              // Hiển thị tên nhân vật
    public AudioClip clickSound;

    private CharacterData selectedCharacter;
    private GameObject currentDisplayObject;

    void Start()
    {
        GenerateCharacterButtons();

        // ✅ Tự động chọn nhân vật đầu tiên được unlock
        foreach (CharacterData data in characters)
        {
            if (data.unlock)
            {
                SelectCharacter(data);
                break;
            }
        }

        playButton.onClick.AddListener(OnPlayButtonPressed);
    }

    void GenerateCharacterButtons()
    {
        foreach (CharacterData data in characters)
        {
            GameObject buttonGO = Instantiate(avatarButtonPrefab, gridParent);

            // Gán sprite avatar
            Transform imageTransform = buttonGO.transform.Find("Image");
            if (imageTransform != null)
            {
                Image avatarImage = imageTransform.GetComponent<Image>();
                if (avatarImage != null)
                    avatarImage.sprite = data.avatar;
            }

            // Tìm LockIcon
            Transform lockTransform = buttonGO.transform.Find("LockIcon");
            if (lockTransform != null)
            {
                lockTransform.gameObject.SetActive(!data.unlock); // ❌ hiện nếu chưa unlock
            }

            // Xử lý Button
            Button buttonComponent = buttonGO.GetComponent<Button>();
            if (buttonComponent != null)
            {
                if (data.unlock)
                {
                    buttonComponent.onClick.AddListener(() => SelectCharacter(data));
                }
                else
                {
                    buttonComponent.interactable = false; // ❌ khóa nút
                }
            }
        }
    }


    void SelectCharacter(CharacterData data)
    {
        SFXManager.Instance.PlaySFX(clickSound);
        selectedCharacter = data;

        if (nameText != null)
            nameText.text = data.name;

        if (currentDisplayObject != null)
            Destroy(currentDisplayObject);

        if (data.characterSelectionPrefab != null)
        {
            currentDisplayObject = Instantiate(data.characterSelectionPrefab, displayParent);
            currentDisplayObject.transform.localPosition = Vector3.zero;
        }
    }

    public void OnPlayButtonPressed()
    {
        SFXManager.Instance.PlaySFX(clickSound);
        
        StartCoroutine(LoadAfterSound());

    }

    private System.Collections.IEnumerator LoadAfterSound()
    {
        float clipLength = clickSound.length;
        yield return new WaitForSeconds(clipLength);
        GameObject soundMnaa = GameObject.Find("AudioManager");
        Destroy(soundMnaa);
        if (selectedCharacter == null)
        {
            Debug.LogWarning("Bạn chưa chọn nhân vật!");
            yield break;
        }

        CharacterSelectionData.Instance.selectedCharacterPrefab = selectedCharacter.characterGameplayPrefab;

        SceneManager.LoadScene("GameScene");
    }
}
