using Newtonsoft.Json.Linq;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private bool endLevelItem;
    private bool _itemPicked;    

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

    public bool ItemPicked { get => _itemPicked; set => _itemPicked = value; }    

    public class SaveDataItemController
    {
        public bool itemPicked;        

        public SaveDataItemController()
        {

        }
        
        public SaveDataItemController(bool itemPicked)
        {
            this.itemPicked = itemPicked;            
        }
    }

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
        if (moveAndFade)
        {
            Move();
            Fade();
        }        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (!moveAndFade && !endLevelItem)
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
        _itemPicked = true;        
        SaveDataGame.instance.ItemsValue = "true";
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

    public JObject Serialize()
    {
        SaveDataItemController saveDataItemController = new SaveDataItemController(this.ItemPicked);
        string jsonString = JsonUtility.ToJson(saveDataItemController);
        JObject returnObject = JObject.Parse(jsonString);
        return returnObject;
    }

    public void DeSerialize(string jsonString)
    {
        SaveDataItemController saveDataItemController = JsonUtility.FromJson<SaveDataItemController>(jsonString);
        this._itemPicked = saveDataItemController.itemPicked;
        if(_itemPicked)
        {
            gameObject.SetActive(false);
        }
    }
}