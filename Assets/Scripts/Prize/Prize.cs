using UnityEngine;

public class Prize : MonoBehaviour
{
    public static string PRIZE_TAG = "Prize";

    public int scoreValue = 1;

    public bool IsGrabbed;

    public Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnGrab(Transform grabAnchor)
    {
        IsGrabbed = true;
        rb.useGravity = false;
        transform.parent = grabAnchor;
        rb.velocity = Vector3.zero;
        rb.freezeRotation = true;
    }

    public void OnRelease()
    {
        IsGrabbed = false;
        transform.parent = null;
        rb.useGravity = true;
        rb.freezeRotation = false;
    }
}
