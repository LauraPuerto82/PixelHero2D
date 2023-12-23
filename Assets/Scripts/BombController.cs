using UnityEngine;

public class BombController : MonoBehaviour
{
    [SerializeField] private float waitForExplode;
    [SerializeField] private float waitForDestroy;
    [SerializeField] private float expansiveWaveRange;
    [SerializeField] private LayerMask isDestroyableLayerMask;

    private Animator myAnimator;
    private Transform myTransform;

    private bool isActive;
    private int IdIsActive;

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        myTransform = GetComponent<Transform>();
        IdIsActive = Animator.StringToHash("isActive");
    }

    private void Update()
    {
        waitForExplode -= Time.deltaTime;
        waitForDestroy -= Time.deltaTime;

        if(waitForExplode <= 0 && !isActive)
        {
            ActivatedBomb();
        }

        if(waitForDestroy <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void ActivatedBomb()
    {
        isActive = true;
        myAnimator.SetBool(IdIsActive, isActive);
        
        Collider2D[] destroyedObjects = Physics2D.OverlapCircleAll(myTransform.position, expansiveWaveRange, isDestroyableLayerMask); 
        
        if(destroyedObjects.Length > 0)
        {
            foreach (var col in destroyedObjects)
            {
                Destroy(col.gameObject);
            }
        }
    }
}