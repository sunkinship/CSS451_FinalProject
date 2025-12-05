using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public enum BodyRotation { None, Left, Right }
public enum ArmDirection { Up, Down }
public enum LeafRotation { Open, Close }

public class ClawController : MonoBehaviour
{
    public static ClawController Instance;

    private const string PRIZE_MASK = "prizeMask";

    [SerializeField] private Transform clawDropLocation;

    [Header("Claw Nodes")]
    [SerializeField] private SceneNode clawBase;
    [SerializeField] private SceneNode clawArm;
    [SerializeField] private SceneNode clawBody;
    [SerializeField] private SceneNode clawLeaf1;
    [SerializeField] private SceneNode clawLeaf2;
    [SerializeField] private SceneNode clawLeaf3;
    [SerializeField] private SceneNode clawEndPt1;
    [SerializeField] private SceneNode clawEndPt2;
    [SerializeField] private Transform grabAnchorLeft;
    [SerializeField] private Transform grabAnchorRight;
    [SerializeField] private Transform prizeSpot;

    [Header("Collision")]
    [SerializeField] private SceneNode claw1SecondPart; //used for check in pushing objects away
    [SerializeField] private SceneNode claw2SecondPart;
    [SerializeField] private CapsuleCollider leftClawCol;
    [SerializeField] private CapsuleCollider rightClawCol;
    [SerializeField] private float pushStrength;
    
    [Header("Claw Settings")]
    [SerializeField] private float clawMoveSpeed = 1f;
    [SerializeField] private float clawExtendSpeed = 1f;
    [SerializeField] private float clawRotateSpeed = 1f;
    [SerializeField] private float clawOpenSpeed = 1f;

    [Header("Claw Transform Bounds")]
    [SerializeField] private Vector2 clawBaseMaxPos;
    [SerializeField] private Vector2 clawBaseMinPos;
    [SerializeField] private Vector2 clawArmExtendMinMax;
    [SerializeField] private Vector2 clawLeaf1RotMinMax;
    [SerializeField] private Vector2 clawLeaf2RotMinMax;

    private Prize grabbedPrize;

    private Vector2 currentBasePos;
    private float currentArmPos;
    private float currentBodyRot;
    private float currentLeaf1Rot;
    private float currentLeaf2Rot;

    private Vector2 startingPos;
    private Vector2 dropPos;

    private Quaternion bodyOriginalRot;
    private Quaternion leaf1OriginalRot;
    private Quaternion leaf2OriginalRot;

    private bool leftLeafHitPrize = false;
    private bool rightLeafHitPrize = false;
    private bool hasRolledDropChance = false;
    private bool prizeShouldDrop = false;
    public bool isCarryingPrize = false;

    public Vector2 CurrentClawMoveDir { get; private set; }
    public BodyRotation CurrentBodyRotDir { get; private set; }

    public bool IsClawDropping => clawDropProcess != null;
    private Coroutine clawDropProcess = null;

    private PrizeDetector leftDetector;
    private PrizeDetector rightDetector;

    public Action OnStartDrop;
    public Action OnEndDrop;

    private void Awake() => Instance = this;

    private void Start()
    {
        currentBasePos = new Vector2(clawBase.NodeOrigin.x, clawBase.NodeOrigin.z);
        currentArmPos = clawArm.NodeOrigin.y;

        currentBodyRot = clawBody.transform.localRotation.y;
        bodyOriginalRot = clawBody.transform.localRotation;

        currentLeaf1Rot = clawLeaf1.transform.localRotation.x;
        currentLeaf2Rot = clawLeaf2.transform.localRotation.x;

        leaf1OriginalRot = clawLeaf1.transform.localRotation;
        leaf2OriginalRot = clawLeaf2.transform.localRotation;

        startingPos = currentBasePos;
        dropPos = new Vector2(clawDropLocation.position.x, clawDropLocation.position.z);

        // Get PrizeDetector components from anchors
        leftDetector = grabAnchorLeft.GetComponent<PrizeDetector>();
        rightDetector = grabAnchorRight.GetComponent<PrizeDetector>();
    }

    private void Update()
    {
        if (CurrentClawMoveDir != Vector2.zero) MoveClawBase(CurrentClawMoveDir);
        if (CurrentBodyRotDir != BodyRotation.None) RotateClawBody(CurrentBodyRotDir);
    }

    void LateUpdate()
    {
        prizeSpot.position = clawLeaf3.LatestWorldMatrix.MultiplyPoint3x4(Vector3.zero);
        grabAnchorLeft.position = clawEndPt1.LatestWorldMatrix.MultiplyPoint3x4(Vector3.zero);
        grabAnchorRight.position = clawEndPt2.LatestWorldMatrix.MultiplyPoint3x4(Vector3.zero);

        leftClawCol.transform.position = claw1SecondPart.LatestWorldMatrix.MultiplyPoint3x4(Vector3.zero);
        rightClawCol.transform.position = claw2SecondPart.LatestWorldMatrix.MultiplyPoint3x4(Vector3.zero);
    }

    #region ACTION
    public void UpdateClawMoveDir(Vector2 dir) => CurrentClawMoveDir = dir;
    public void UpdateBodyRotDir(BodyRotation dir) => CurrentBodyRotDir = dir;

    public void StartDropProcess()
    {
        if (IsClawDropping)
        {
            Debug.LogWarning("Already dropping claw");
            return;
        }
        ResetClawFlags();

        OnStartDrop?.Invoke();
        clawDropProcess = StartCoroutine(ClawDropProcess());
    }
    #endregion

    #region DROP SEQUENCE
    private IEnumerator ClawDropProcess()
    {
        while (OpenClaws()) yield return null;

        while (LowerArm())
        {
            CheckBlockingObjects();
            yield return null;
        }
        while (CloseClaws())
        {
            CheckBlockingObjects();
            yield return null;
        }

        CheckGrab();
        hasRolledDropChance = true;
        //prizeShouldDrop = (UnityEngine.Random.Range(0, 3) == 0);
        
        while (RaiseArm()) yield return null;

        if (isCarryingPrize == false)
        {
            while (CloseClaws()) yield return null;
            clawDropProcess = null;
            OnEndDrop?.Invoke();
            yield break;
        }

        yield return AutoMoveToPosition(dropPos);

        if (grabbedPrize != null)
        {
            grabbedPrize.OnRelease();
            grabbedPrize = null;
        }
        ResetClawFlags();
        while (OpenClaws()) yield return null;

        yield return AutoMoveToPosition(startingPos);
        while (CloseClaws()) yield return null;

        clawDropProcess = null;
        OnEndDrop?.Invoke();
    }
    #endregion

    #region CLAMPED TRANSFORMS
    private void MoveClawBase(Vector2 direction)
    {
        direction *= clawMoveSpeed;
        float targetX = Mathf.Clamp(currentBasePos.x + direction.x * Time.deltaTime, clawBaseMinPos.x, clawBaseMaxPos.x);
        float targetZ = Mathf.Clamp(currentBasePos.y + direction.y * Time.deltaTime, clawBaseMinPos.y, clawBaseMaxPos.y);

        NodeTransformer.TranslateNode(clawBase, targetX, Axis.X);
        NodeTransformer.TranslateNode(clawBase, targetZ, Axis.Z);

        currentBasePos = new Vector2(targetX, targetZ);
    }

    private IEnumerator AutoMoveToPosition(Vector2 targetPos)
    {
        if (isCarryingPrize && prizeShouldDrop) DropPrize();
        Vector2 moveDir = (targetPos - currentBasePos).normalized;
        while (Vector2.Distance(currentBasePos, targetPos) > 0.05f)
        {
            MoveClawBase(moveDir);
            yield return null;
        }
    }

    private bool ExtendClawArm(ArmDirection direction)
    {
        float directionValue = direction == ArmDirection.Up ? 1 : -1;
        directionValue *= clawExtendSpeed;

        float targetY = currentArmPos + directionValue * Time.deltaTime;

        targetY = Mathf.Clamp(targetY, clawArmExtendMinMax.x, clawArmExtendMinMax.y);

        NodeTransformer.TranslateNode(clawArm, targetY, Axis.Y);
        currentArmPos = targetY;
        return targetY != clawArmExtendMinMax.x && targetY != clawArmExtendMinMax.y;
    }

    private void RotateClawBody(BodyRotation direction)
    {
        float rotValue = direction == BodyRotation.Left ? -1 : 1;
        float targetRot = currentBodyRot + rotValue * clawRotateSpeed * Time.deltaTime;

        NodeTransformer.RotateNode(clawBody, targetRot, bodyOriginalRot, Axis.Y, RotationSpace.Local);
        currentBodyRot = targetRot;
    }
        
    private bool RotateClawLeafs(LeafRotation direction) =>
        RotateClawLeaf1(direction) && RotateClawLeaf2(direction);

    private bool RotateClawLeaf1(LeafRotation direction)
    {
        if (direction == LeafRotation.Close && leftDetector.IsTouchingPrize)
        {
            leftLeafHitPrize = true;
            return false;
        }

        float rotValue = direction == LeafRotation.Open ? 1 : -1;
        float targetRot = Mathf.Clamp(currentLeaf1Rot + rotValue * clawOpenSpeed * Time.deltaTime, clawLeaf1RotMinMax.x, clawLeaf1RotMinMax.y);

        NodeTransformer.RotateNode(clawLeaf1, targetRot, leaf1OriginalRot, Axis.Z, RotationSpace.Local);
        currentLeaf1Rot = targetRot;
        return targetRot != clawLeaf1RotMinMax.x && targetRot != clawLeaf1RotMinMax.y;
    }

    private bool RotateClawLeaf2(LeafRotation direction)
    {
        if (direction == LeafRotation.Close && rightDetector.IsTouchingPrize)
        {
            rightLeafHitPrize = true;
            return false;
        }

        float rotValue = direction == LeafRotation.Open ? -1 : 1;
        float targetRot = Mathf.Clamp(currentLeaf2Rot + rotValue * clawOpenSpeed * Time.deltaTime, clawLeaf2RotMinMax.x, clawLeaf2RotMinMax.y);

        NodeTransformer.RotateNode(clawLeaf2, targetRot, leaf2OriginalRot, Axis.Z, RotationSpace.Local);
        currentLeaf2Rot = targetRot;
        return targetRot != clawLeaf2RotMinMax.x && targetRot != clawLeaf2RotMinMax.y;
    }

    private bool OpenClaws() => RotateClawLeafs(LeafRotation.Open);
    private bool CloseClaws()
    {
        bool leaf1Moving = RotateClawLeaf1(LeafRotation.Close);
        bool leaf2Moving = RotateClawLeaf2(LeafRotation.Close);
        return leaf1Moving || leaf2Moving;
    }

    private bool RaiseArm() => ExtendClawArm(ArmDirection.Up);
    private bool LowerArm() => ExtendClawArm(ArmDirection.Down);
    #endregion

    #region PRIZE
    private void CheckGrab()
    {
        if (!leftLeafHitPrize || !rightLeafHitPrize)
        {
            return;
        }

        Prize leftPrize = leftDetector.CurrentPrize;
        Prize rightPrize = rightDetector.CurrentPrize;

        if (leftPrize == null || rightPrize == null)
        {
            return;
        }

        if (leftPrize != rightPrize)
        {
            return;
        }

        grabbedPrize = leftPrize;
        grabbedPrize.OnGrab(prizeSpot);
        isCarryingPrize = true;
    }

    private void DropPrize()
    {
        if (grabbedPrize != null)
        {
            grabbedPrize.transform.SetParent(null);
            Rigidbody rb = grabbedPrize.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }

            isCarryingPrize = false;
            prizeShouldDrop = false;
            hasRolledDropChance = false;
        }
    }
    #endregion

    #region COLLISION
    private void CheckBlockingObjects()
    {
        PushBlockingObjects(leftClawCol, claw1SecondPart.LatestWorldMatrix.MultiplyPoint3x4(Vector3.zero));
        PushBlockingObjects(rightClawCol, claw2SecondPart.LatestWorldMatrix.MultiplyPoint3x4(Vector3.zero));
    }

    private void PushBlockingObjects(CapsuleCollider collider, Vector3 targetPos) //push objects blocking claw away
    {
        Collider[] hits = OverlapCapsuleFromCollider(collider);

        foreach (Collider hit in hits)
        {
            Collider clawCol = collider;

            if (Physics.ComputePenetration(clawCol, targetPos, transform.rotation,
                    hit, hit.transform.position, hit.transform.rotation,
                    out Vector3 dir, out float distance))
            {
                Vector3 pushVector = dir * distance;

                Rigidbody rb = hit.attachedRigidbody;
                pushVector *= 0.95f;

                if (rb)
                {
                    rb.AddForce(pushVector * 5, ForceMode.VelocityChange);
                }
            }
        }
    }

    public static Collider[] OverlapCapsuleFromCollider(CapsuleCollider collider) //check for col overlap
    {
        Transform t = collider.transform;

        float radius = collider.radius * Mathf.Max(
            Mathf.Abs(t.lossyScale.x),
            Mathf.Abs(t.lossyScale.y),
            Mathf.Abs(t.lossyScale.z)
        );

        float height = collider.height * Mathf.Abs(t.lossyScale.y);

        int direction = collider.direction;

        Vector3 center = t.TransformPoint(collider.center);

        float halfHeight = Mathf.Max(height * 0.5f - radius, 0f);

        Vector3 dir;
        switch (direction)
        {
            case 0:
                dir = t.right;
                break;
            case 1:
                dir = t.up;
                break;
            default:
                dir = t.forward;
                break;
        }

        Vector3 point1 = center + dir * halfHeight;
        Vector3 point2 = center - dir * halfHeight;

        Collider[] results = Physics.OverlapCapsule(point1, point2, radius, 1 << LayerMask.NameToLayer(PRIZE_MASK));

        return results;
    }
    #endregion

    private void ResetClawFlags()
    {
        CurrentClawMoveDir = Vector2.zero;
        CurrentBodyRotDir = BodyRotation.None;

        leftLeafHitPrize = false;
        rightLeafHitPrize = false;
    }
}
