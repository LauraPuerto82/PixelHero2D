using UnityEngine;

public class FrameRateController : MonoBehaviour
{
    [SerializeField] private int target;

    private void Awake()
    {
        Application.targetFrameRate = target;
    }
}