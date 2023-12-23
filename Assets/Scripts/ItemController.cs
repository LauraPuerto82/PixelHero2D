using UnityEngine;

public enum ItemType
{
    None,
    Heart,
    SpinningCoin,
    ShiningCoin    
}

public class ItemController : MonoBehaviour
{            
    [SerializeField] private ItemType itemType;
    [SerializeField] private bool endLevelItem;
    
    private Transform myTransform;    

    // Move and Fade
    [SerializeField] private float moveFadeTime;
    private float elapsedTime = 0;
    private float intervalTime;
    private bool moveAndFade = false;    
    // move
    private Vector3 targetPosition;
    private float yOffset = 2f;    
    // fade
    float desiredAlpha;
    float currentAlpha;    
    private Color currentColor;
    private SpriteRenderer mySpriteRenderer;

    private void Awake()
    {
        myTransform = GetComponent<Transform>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();        
    }

    private void Start()
    {        
        targetPosition = new Vector3(myTransform.position.x, myTransform.position.y + yOffset, myTransform.position.z);        
        currentAlpha = mySpriteRenderer.color.a;
        desiredAlpha = 0;
    }

    private void Update()
    {
        if(moveAndFade)
        {
            Move();
            Fade();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {       
            if(!moveAndFade && !endLevelItem)
                ObjectPicked();

            if (endLevelItem)
                GameManager.instance.LevelPassed();
        }        
    }
    
    private void ObjectPicked()
    {        
        ItemsManager.instance.Items[itemType.ToString()]++;
        ItemsManager.instance.SetTracker();
        moveAndFade = true;
        Destroy(gameObject, moveFadeTime);
    }   

    private void Move()
    {
        elapsedTime += Time.deltaTime;
        intervalTime = elapsedTime / moveFadeTime;                
        myTransform.position = Vector3.Lerp(myTransform.position, targetPosition, intervalTime);                
    }

    private void Fade()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, intervalTime);
        currentColor = new Color(1f, 1f, 1f, currentAlpha);
        mySpriteRenderer.color = currentColor;
    }
}