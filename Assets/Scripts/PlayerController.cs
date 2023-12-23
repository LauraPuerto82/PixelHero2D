using UnityEngine;

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
    // Player animation
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
    private float dashCounter;
    [SerializeField] private float waitForDash;
    private float afterDashCounter;

    [Header("Player Dash After Image")]
    [SerializeField] private SpriteRenderer mySpriteRenderer;
    [SerializeField] private SpriteRenderer afterImageSpriteRenderer;
    [SerializeField] private float afterImageLifetime;
    [SerializeField] private Color afterImageColor;
    [SerializeField] private float afterImageTimeBetween;
    private float afterImageCounter;

    // Player Extras
    private PlayerExtrasTracker playerExtrasTracker;
    

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody2D>();     
        myTransform = GetComponent<Transform>();
        playerExtrasTracker = GetComponent<PlayerExtrasTracker>();
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
        Shoot();      
        PlayDust();
        BallMode();
    }

    private void Dash()
    {
        if (afterDashCounter > 0)
            afterDashCounter -= Time.deltaTime;
        else
        {
            if ((Input.GetButtonDown("Fire2") && standingPlayer.activeSelf) && playerExtrasTracker.CanDash)
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
        float inputX = Input.GetAxisRaw("Horizontal") * moveSpeed;        
        myRigidbody.velocity = new Vector2(inputX, myRigidbody.velocity.y);

        if (standingPlayer.activeSelf)
            animatorStandingPlayer.SetFloat(IdSpeed, Mathf.Abs(myRigidbody.velocity.x));
        if(ballPlayer.activeSelf)
            animatorBallPlayer.SetFloat(IdSpeed, Mathf.Abs(myRigidbody.velocity.x));
    }

    private void Jump()
    {
        float overlapCircleRadio = 0.2f;
        //float raycastDistance = 0.2f;
        isGrounded = Physics2D.OverlapCircle(checkGroundPoint.position, overlapCircleRadio, selectedLayerMask);
        //isGrounded = Physics2D.Raycast(checkGroundPoint.position, Vector2.down, raycastDistance, selectedLayerMask);                

        if (Input.GetButtonDown("Jump") && (isGrounded || (canDoubleJump && playerExtrasTracker.CanDoubleJump)))
        {
            if (isGrounded)
            {
                canDoubleJump = true;
                Instantiate(dustJump, transformDustPoint.position, Quaternion.identity);
            }
            else
            {
                canDoubleJump = false;
                animatorStandingPlayer.SetTrigger(IdCanDoubleJump);
                
            }
            
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        }
       
        animatorStandingPlayer.SetBool(IdIsGrounded, isGrounded);
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

    void Shoot()
    {
        if (Input.GetButtonDown("Fire1") && standingPlayer.activeSelf)
        {
            ArrowController tempArrowController = Instantiate(arrowController, transformArrowPoint.position, transformArrowPoint.rotation);
            tempArrowController.ArrowDirection = new Vector2(myTransform.localScale.x, 0f);
            tempArrowController.GetComponent<SpriteRenderer>().flipX = isFlippedInX;
            animatorStandingPlayer.SetTrigger(IdShootArrow);
        }
        if ((Input.GetButtonDown("Fire1") && ballPlayer.activeSelf) && playerExtrasTracker.CanDropBombs)
            Instantiate(prefabBomb, transformBombPoint.position, Quaternion.identity);
    } 

    private void PlayDust()
    {
        if(myRigidbody.velocity.x != 0 && isIdle)
        {
            isIdle = false;
            if (isGrounded)
                Instantiate(dustJump, transformDustPoint.position, Quaternion.identity);
        }
        if (myRigidbody.velocity.x == 0)
            isIdle = true;
    }

    private void BallMode()
    {
        float inputVertical = Input.GetAxisRaw("Vertical");
        if(((inputVertical <= -.9 && !ballPlayer.activeSelf) || (inputVertical >= .9 && ballPlayer.activeSelf)) && playerExtrasTracker.CanEnterBallMode)
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
}