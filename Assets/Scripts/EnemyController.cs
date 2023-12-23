using UnityEngine;

public class EnemyController : MonoBehaviour
{    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.GameOver();            
        }
    }         
}