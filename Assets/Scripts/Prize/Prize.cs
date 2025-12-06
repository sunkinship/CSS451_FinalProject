using System.Collections.Generic;
using UnityEngine;

public class Prize : MonoBehaviour
{
    public static string PRIZE_TAG = "Prize";

    public int scoreValue = 1;

    public bool IsGrabbed;

    public Rigidbody rb;

    public List<Transform> colliderRegions; //used to check how much of object claw has grabbed to determine drop chance


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

    public float GetOverlapPercent(Collider clawTrigger)
    {
        if (colliderRegions == null || colliderRegions.Count == 0)
            return 1;

        int insideCount = 0;

        for (int i = 0; i < colliderRegions.Count; i++)
        {
            if (IsPointInsideCollider(clawTrigger, colliderRegions[i].position))
            {
                insideCount++;
            }
        }
        //Debug.Log($"Overlap {(float)insideCount / colliderRegions.Count * 100}");
        return (float)insideCount / colliderRegions.Count;
    }

    private static bool IsPointInsideCollider(Collider col, Vector3 point)
    {
        Vector3 closest = col.ClosestPoint(point);
        return (closest == point);
    }
}
