using UnityEngine;

public class CameraController : MonoBehaviour
{   
    private PlayerController playerController;
    private Transform myTransform;
    private Transform playerTransform;
    private BoxCollider2D levelLimit;

    private float cameraHorizontalSize;
    private float cameraVerticalSize;

    void Start()
    {
        playerController = GameObject.FindObjectOfType<PlayerController>();        
        playerTransform = playerController.transform;
        myTransform = GetComponent<Transform>();
        levelLimit = GameObject.Find("LevelLimit").GetComponent<BoxCollider2D>();

        cameraVerticalSize = Camera.main.orthographicSize;
        cameraHorizontalSize = Camera.main.orthographicSize * Camera.main.aspect;
    }
    
    void Update()
    {
        if (playerController != null)
        {
            float minXLimit = levelLimit.bounds.min.x + cameraHorizontalSize;
            float maxXLimit = levelLimit.bounds.max.x - cameraHorizontalSize;

            float minYLimit = levelLimit.bounds.min.y + cameraVerticalSize;
            float maxYLimit = levelLimit.bounds.max.y - cameraVerticalSize;

            float x = Mathf.Clamp(playerTransform.position.x, minXLimit, maxXLimit);
            float y = Mathf.Clamp(playerTransform.position.y, minYLimit, maxYLimit);
            float z = myTransform.position.z;

            myTransform.position = new Vector3(x, y, z);
        }   
    }
}