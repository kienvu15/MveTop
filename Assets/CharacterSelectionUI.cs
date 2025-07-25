using System.Collections.Generic;
using TMPro;
using UnityEditor.U2D.Animation;
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

    private CharacterData selectedCharacter;
    private GameObject currentDisplayObject;

    void Start()
    {
        GenerateCharacterButtons();
        playButton.onClick.AddListener(OnPlayButtonPressed);
    }

    void GenerateCharacterButtons()
    {
        foreach (CharacterData data in characters)
        {
            GameObject buttonGO = Instantiate(avatarButtonPrefab, gridParent);

            // Gán sprite vào Image con (nên đặt tên GameObject là "Image" trong prefab)
            Transform imageTransform = buttonGO.transform.Find("Image");
            if (imageTransform != null)
            {
                Image avatarImage = imageTransform.GetComponent<Image>();
                if (avatarImage != null)
                {
                    avatarImage.sprite = data.avatar;
                }
                else
                {
                    Debug.LogWarning("Image component không tồn tại trên GameObject 'Image'");
                }
            }
            else
            {
                Debug.LogWarning("Không tìm thấy GameObject tên 'Image' trong avatarButtonPrefab");
            }

            // Gán sự kiện click
            Button buttonComponent = buttonGO.GetComponent<Button>();
            if (buttonComponent != null)
            {
                buttonComponent.onClick.AddListener(() => SelectCharacter(data));
            }
            else
            {
                Debug.LogWarning("Không tìm thấy Button component trên avatarButtonPrefab");
            }
        }
    }


    void SelectCharacter(CharacterData data)
    {
        selectedCharacter = data;

        // Hiển thị tên
        if (nameText != null)
            nameText.text = data.name;

        // Xóa nhân vật hiển thị trước đó
        if (currentDisplayObject != null)
            Destroy(currentDisplayObject);

        // Tạo nhân vật demo (hiển thị)
        if (data.characterSelectionPrefab != null)
        {
            currentDisplayObject = Instantiate(data.characterSelectionPrefab, displayParent);
            currentDisplayObject.transform.localPosition = Vector3.zero;
        }
    }

    public void OnPlayButtonPressed()
    {
        if (selectedCharacter == null)
        {
            Debug.LogWarning("Bạn chưa chọn nhân vật!");
            return;
        }

        // Lưu prefab gameplay
        CharacterSelectionData.Instance.selectedCharacterPrefab = selectedCharacter.characterGameplayPrefab;

        // Chuyển scene
        SceneManager.LoadScene("GameScene"); // Đổi tên tùy bạn
    }
}
