using UnityEngine;

public class MiniCamController : MonoBehaviour
{
    [Header("Target to Follow")]
    [SerializeField] private Transform target;

    [Header("Camera Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 5, 0);
    private void LateUpdate()
    {
        Vector3 desiredPos = target.position + offset;
        transform.position = desiredPos;

    }
}
