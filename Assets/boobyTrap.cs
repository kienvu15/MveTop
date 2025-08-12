using UnityEngine;

public class boobyTrap : MonoBehaviour
{
    public Sprite activeSprite;
    public Sprite inactiveSprite;
    public Trap Trap;

    public float timer = 0f;
    public float timerCounter = 1f;
    public float resetTimer = 1f;
    public float retime = 0f;

    public bool isActive = false;
    public bool resetTrap = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Trap.GetComponent<Trap>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isActive == true)
        {
            ActivateTrap();
        }

        if (resetTrap == true)
        {
            retime += Time.deltaTime;
            if (retime >= resetTimer)
            {
                GetComponent<SpriteRenderer>().sprite = inactiveSprite;
                Trap.isActive = false;
                resetTrap = false; 
                retime = 0f;
            }
           
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isActive = true;
        }
    }
    
    void  ActivateTrap()
    {
        timer += Time.deltaTime;
        if(timer >= timerCounter)
        {
            
            GetComponent<SpriteRenderer>().sprite = activeSprite;
            Trap.isActive = true;
            timer = 0f;
            isActive = false;
            resetTrap = true;

            //Invoke(nameof(EnableTrap), 1f);
        }
        
    }

    void EnableTrap()
    {
        Trap.enabled = true;
    }
}
