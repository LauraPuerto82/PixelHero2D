using UnityEngine;

public class ArrowController : MonoBehaviour
{

    [SerializeField] private float arrowSpeed;
    [SerializeField] private GameObject arrowImpact;
    [SerializeField] private GameObject batImpact;

    private Rigidbody2D myRigidBody;
    private Transform myTransform;
    
    private Vector2 _arrowDirection;

    public Vector2 ArrowDirection { get => _arrowDirection; set => _arrowDirection = value; }

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody2D>();  
        myTransform = GetComponent<Transform>();
    }    

    void Update()
    {
        myRigidBody.velocity = ArrowDirection * arrowSpeed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bat"))
        {
            Instantiate(batImpact, other.transform.position, Quaternion.identity);
            Destroy(other.gameObject);
        }
        else
        {
            Instantiate(arrowImpact, myTransform.position, Quaternion.identity);            
        }
        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}