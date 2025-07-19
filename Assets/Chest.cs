using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    public bool isOpened { get; private set; }
    public string chestID { get; private set; }

    public GameObject itemPrefab;
    public Sprite OpenSprite;
    void Start()
    {
        chestID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        if(!CanInteract())
            return;
        OpenChest();
    }
   
    public bool CanInteract()
    {
        return !isOpened;
    }

    private void OpenChest() 
    { 
        SetOpened(true);

        if (itemPrefab)
        {
            GameObject droppedItem = Instantiate(itemPrefab, transform.position + Vector3.down, Quaternion.identity);
            droppedItem.GetComponent<BounceEffect>().StartBounce();


        }
    }

    public void SetOpened(bool opened)
    {
        if (isOpened = opened)
        {
            GetComponent<SpriteRenderer>().sprite = OpenSprite;
        }
    }
}
