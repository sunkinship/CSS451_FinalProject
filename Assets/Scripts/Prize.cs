using UnityEngine;

public class Prize : MonoBehaviour
{
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
    }

    public void OnRelease()
    {
        IsGrabbed = false;
        transform.parent=null;
        rb.useGravity = true;
    }
}
