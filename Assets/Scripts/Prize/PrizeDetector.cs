using UnityEngine;

public class PrizeDetector : MonoBehaviour
{
    public Prize CurrentPrize { get; private set; }
    public bool IsTouchingPrize => CurrentPrize != null;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Prize")) CurrentPrize = other.GetComponent<Prize>();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Prize") && CurrentPrize != null)
            CurrentPrize = null;
    }
}
