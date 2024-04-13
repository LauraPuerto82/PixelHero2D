using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Player sprites
    private GameObject standingPlayer;
    private GameObject ballPlayer;

    [Header("Player Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask selectedLayerMask;
    private Rigidbody2D myRigidbody;    
    private Transform checkGroundPoint;    
    private bool isGrounded, isFlippedInX;
    private float ballModeCounter;
    [SerializeField] private float waitForBallMode;
    private Vector2 initialPos;
    private bool tryToJump;


    [Header(" Player Animation ")]
    private Animator animatorStandingPlayer;
    private Animator animatorBallPlayer;
    private int IdSpeed, IdIsGrounded, IdShootArrow, IdCanDoubleJump;

    [Header("Player Shoot")]
    [SerializeField] private ArrowController arrowController;
    private Transform transformArrowPoint, myTransform;
    private Transform transformBombPoint;
    [SerializeField] private GameObject prefabBomb;

    [Header("Player Dust")]
    [SerializeField] private GameObject dustJump;
    private Transform transformDustPoint;
    private bool isIdle, canDoubleJump;

    [Header("Player Dash")]
    [SerializeField] private float dashSpeed;
    [SerializeField] private float dashTime;
    [SerializeField] private float waitForDash;
    private float dashCounter;    
    private float afterDashCounter;
    private bool tryToDash;

    [Header("Player Dash After Image")]
    [SerializeField] private SpriteRenderer mySpriteRenderer;
    [SerializeField] private SpriteRenderer afterImageSpriteRenderer;
    [SerializeField] private float afterImageLifetime;
    [SerializeField] private Color afterImageColor;
    [SerializeField] private float afterImageTimeBetween;
    private float afterImageCounter;

    [Header(" Player Input ")]
    private PlayerInput playerInput;

    [Header(" Action Maps ")]
    private InputActionMap playerNormal;
    private InputActionMap playerAlternative;
    
    [Header(" Input Actions ")]
    private InputAction moveAction;
    private InputAction shootAction;    
    private InputAction dashAction;    
    private InputAction jumpAction;    
    private InputAction ballModeAction;    
    private InputAction switchMapAction;        

    // Player Extras
    private PlayerExtrasTracker playerExtrasTracker;

    public bool IsGrounded { get => isGrounded; set => isGrounded = value; }   

    public class SaveDataPlayerController
    {
        public Vector2 initialPos;
        public Vector2 finalPos;
        public bool isFlippedInX;

        public SaveDataPlayerController()
        {

        }

        public SaveDataPlayerController(Vector2 initialPos, Vector2 finalPos, bool isFlippedInX)
        {
            this.initialPos = initialPos;
            this.finalPos = finalPos;
            this.isFlippedInX = isFlippedInX;
        }
    }

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();     
        myTransform = GetComponent<Transform>();
        initialPos = myTransform.position;
        playerExtrasTracker = GetComponent<PlayerExtrasTracker>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {       
        playerNormal = playerInput.actions.FindActionMap("PlayerNormal");
        playerAlternative = playerInput.actions.FindActionMap("PlayerAlternative");

        ActivatePlayerNormal();
    }

    private void Start()
    {     
        standingPlayer = GameObject.Find("StandingPlayer");
        ballPlayer = GameObject.Find("BallPlayer");
        ballPlayer.SetActive(false);
        checkGroundPoint = GameObject.Find("CheckGroundPoint").transform;
        animatorStandingPlayer = standingPlayer.GetComponent<Animator>();
        animatorBallPlayer = ballPlayer.GetComponent<Animator>();
        transformArrowPoint = GameObject.Find("ArrowPoint").GetComponent<Transform>();
        transformDustPoint = GameObject.Find("DustPoint").GetComponent<Transform>();
        transformBombPoint = GameObject.Find("BombPoint").GetComponent <Transform>();
        IdSpeed = Animator.StringToHash("speed");
        IdIsGrounded = Animator.StringToHash("isGrounded");
        IdShootArrow = Animator.StringToHash("shootArrow");
        IdCanDoubleJump = Animator.StringToHash("canDoubleJump");
    }

    private void Update()
    {        
        Dash();
        Jump();
        CheckAndSetDirection();               
        PlayDust();
        BallMode();        
    }

    private void Dash()
    {
        if (afterDashCounter > 0)
            afterDashCounter -= Time.deltaTime;
        else
        {            
            if (standingPlayer.activeSelf && playerExtrasTracker.CanDash && tryToDash)
            {
                dashCounter = dashTime;
                ShowAfterImage();
            }
        }

        if(dashCounter > 0)
        {
            dashCounter -= Time.deltaTime;
            myRigidbody.velocity = new Vector2(dashSpeed * myTransform.localScale.x, myRigidbody.velocity.y);            
            afterImageCounter -= Time.deltaTime;
            if(afterImageCounter <= 0)
            {
                ShowAfterImage();
            }
            afterDashCounter = waitForDash;
        }
        else
        {
            Move();
        }        
    }
    
    private void Move()
    {        
        float inputX = moveAction.ReadValue<float>() * moveSpeed;
        Debug.Log("InputX is: " + inputX);

        myRigidbody.velocity = new Vector2(inputX, myRigidbody.velocity.y);

        if (standingPlayer.activeSelf)
            animatorStandingPlayer.SetFloat(IdSpeed, Mathf.Abs(myRigidbody.velocity.x));
        if(ballPlayer.activeSelf)
            animatorBallPlayer.SetFloat(IdSpeed, Mathf.Abs(myRigidbody.velocity.x));
    }

    private void Jump()
    {
        float overlapCircleRadio = 0.2f;        
        IsGrounded = Physics2D.OverlapCircle(checkGroundPoint.position, overlapCircleRadio, selectedLayerMask);        

        if (tryToJump && (IsGrounded || (canDoubleJump && playerExtrasTracker.CanDoubleJump)))
        {
            if (IsGrounded)
            {
                canDoubleJump = true;
                Instantiate(dustJump, transformDustPoint.position, Quaternion.identity);
                tryToJump = false;
            }
            else
            {
                canDoubleJump = false;
                animatorStandingPlayer.SetTrigger(IdCanDoubleJump);
                tryToJump = false;
            }

            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);            
        }
                
        animatorStandingPlayer.SetBool(IdIsGrounded, IsGrounded);
    }    
    
    private void CheckAndSetDirection()
    {
        if (myRigidbody.velocity.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            isFlippedInX = true;
        }
        else if (myRigidbody.velocity.x > 0)
        {
            transform.localScale = Vector3.one;
            isFlippedInX = false;
        }        
    }

    private void Shoot(InputAction.CallbackContext context)
    {                
        if (standingPlayer.activeSelf)
        {
            ArrowController tempArrowController = Instantiate(arrowController, transformArrowPoint.position, transformArrowPoint.rotation);
            tempArrowController.ArrowDirection = new Vector2(myTransform.localScale.x, 0f);
            tempArrowController.GetComponent<SpriteRenderer>().flipX = isFlippedInX;
            animatorStandingPlayer.SetTrigger(IdShootArrow);
        }       
        if (ballPlayer.activeSelf && playerExtrasTracker.CanDropBombs)
            Instantiate(prefabBomb, transformBombPoint.position, Quaternion.identity);
    }   

    private void PlayDust()
    {
        if(myRigidbody.velocity.x != 0 && isIdle)
        {
            isIdle = false;
            if (IsGrounded)
                Instantiate(dustJump, transformDustPoint.position, Quaternion.identity);
        }
        if (myRigidbody.velocity.x == 0)
            isIdle = true;
    }

    private void BallMode()
    {        
        float inputVertical = ballModeAction.ReadValue<float>();

        if (((inputVertical <= -.9 && !ballPlayer.activeSelf) || (inputVertical >= .9 && ballPlayer.activeSelf)) && playerExtrasTracker.CanEnterBallMode)
        {
            ballModeCounter -= Time.deltaTime;
            if (ballModeCounter < 0)
            {
                ballPlayer.SetActive(!ballPlayer.activeSelf);
                standingPlayer.SetActive(!standingPlayer.activeSelf);
            }            
        }
        else
            ballModeCounter = waitForBallMode;
    }

    private void ShowAfterImage()
    {
        SpriteRenderer afterImage = Instantiate(afterImageSpriteRenderer, myTransform.position, myTransform.rotation);
        afterImage.sprite = mySpriteRenderer.sprite;
        afterImage.transform.localScale = myTransform.localScale;
        afterImage.color = afterImageColor;
        Destroy(afterImage.gameObject, afterImageLifetime);
        afterImageCounter = afterImageTimeBetween;
    }    

    private void StartDash(InputAction.CallbackContext context) => tryToDash = true;

    private void StopDash(InputAction.CallbackContext context) => tryToDash = false;    

    private void StartJump(InputAction.CallbackContext context) => tryToJump = true;    

    private void StopJump(InputAction.CallbackContext context) => tryToJump = false;

    private void SwitchActionMap(InputAction.CallbackContext context)
    {
        if (playerInput.currentActionMap == playerNormal)
            ActivatePlayerAlternative();
        else if (playerInput.currentActionMap == playerAlternative || playerInput.currentActionMap == null)
            ActivatePlayerNormal();
    }

    private void ActivatePlayerNormal()
    {
        Debug.Log("Player Normal activo");
        playerInput.SwitchCurrentActionMap("playerNormal");
        DesuscribeActions();
        FindActions(playerNormal);        
        SuscribeActions();
    }

    private void ActivatePlayerAlternative()
    {
        Debug.Log("Player Alternative activo");
        playerInput.SwitchCurrentActionMap("playerAlternative");
        DesuscribeActions();
        FindActions(playerAlternative);        
        SuscribeActions();
    }

    private void FindActions(InputActionMap map)
    {
        moveAction = map.FindAction("Move");
        shootAction = map.FindAction("Shoot");
        dashAction = map.FindAction("Dash");
        jumpAction = map.FindAction("Jump");
        ballModeAction = map.FindAction("BallMode");
        switchMapAction = map.FindAction("SwitchMap");
    }

    private void SuscribeActions()
    {
        shootAction.performed += Shoot;

        dashAction.performed += StartDash;
        dashAction.canceled += StopDash;

        jumpAction.started += StartJump;
        jumpAction.canceled += StopJump;

        switchMapAction.performed += SwitchActionMap;
    }

    private void DesuscribeActions()
    {
        if(shootAction != null)
            shootAction.performed -= Shoot;

        if (dashAction != null)
        {
            dashAction.performed -= StartDash;
            dashAction.canceled -= StopDash;
        }

        if (jumpAction != null)
        {
            jumpAction.started -= StartJump;
            jumpAction.canceled -= StopJump;
        }

        if(switchMapAction != null)
            switchMapAction.performed -= SwitchActionMap;
    }

    public JObject Serialize()
    {
        SaveDataPlayerController saveDataPlayerController = new SaveDataPlayerController(initialPos, transform.position, isFlippedInX);
        string jsonString = JsonUtility.ToJson(saveDataPlayerController);
        JObject returnObject = JObject.Parse(jsonString);
        return returnObject;
    }

    public void DeSerialize(string jsonString)
    {
        SaveDataPlayerController saveDataPlayerController = JsonUtility.FromJson<SaveDataPlayerController>(jsonString);
        this.transform.position = saveDataPlayerController.finalPos;
        this.isFlippedInX = saveDataPlayerController.isFlippedInX;
        if (isFlippedInX)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = Vector3.one;
        }
    }

    private void OnDisable()
    {       
        DesuscribeActions();            
    }
}