using System;
using System.Collections;
using UnityEngine;

public enum ArmDirection { Up, Down }
public enum LeafRotation { Open, Close }

//control claw using NodeTransformer functions
//functions called by UI button inputs
public class ClawController : MonoBehaviour
{
    public static ClawController Instance;

    [Header("Claw Nodes")]
    [SerializeField] private SceneNode clawBase;
    [SerializeField] private SceneNode clawArm;
    [SerializeField] private SceneNode clawLeaf1;
    [SerializeField] private SceneNode clawLeaf2;

    [Header("Claw Settings")]
    [SerializeField] private float clawMoveSpeed = 1;
    [SerializeField] private float clawExtendSpeed = 1;
    [SerializeField] private float clawOpenSpeed = 1;
    [SerializeField] private Transform clawDropLocation; //location above prize drop hole

    [Header("Claw Transform Bounds")]
    [SerializeField] private Vector2 clawBaseMaxPos; //max and min positions to move on XZ axis
    [SerializeField] private Vector2 clawBaseMinPos;

    [SerializeField] private Vector2 clawArmExtendMinMax; //x is min scale, y is max scale

    [SerializeField] private Vector2 clawLeaf1RotMinMax; //x in min, y is max rotation on axis
    [SerializeField] private Vector2 clawLeaf2RotMinMax;

    //track current transfomrations to be able to clamp
    private Vector2 currentBasePos;
    private float currentArmPos;
    private float currentLeaf1Rot;
    private float currentLeaf2Rot;

    //position for claw to return to after finishing claw drop process
    private Vector2 startingPos;
    private Vector2 dropPos; //get x and z from drop pos area transform

    //track original rotations for rotation calculation
    private Quaternion leaf1OriginalRot;
    private Quaternion leaf2OriginalRot;

    public Vector2 CurrentClawMoveDir { get; private set; } //current move vector

    public bool IsClawDropping => clawDropProcess != null;
    private Coroutine clawDropProcess = null;

    public Action OnStartDrop;
    public Action OnEndDrop;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentBasePos = new Vector2(clawBase.NodeOrigin.x, clawBase.NodeOrigin.z);
        currentArmPos = clawArm.NodeOrigin.y;

        currentLeaf1Rot = clawLeaf1.transform.localRotation.x;
        currentLeaf2Rot = clawLeaf2.transform.localRotation.x;

        leaf1OriginalRot = clawLeaf1.transform.localRotation;
        leaf2OriginalRot = clawLeaf2.transform.localRotation;

        startingPos = currentBasePos;
        dropPos = new Vector2(clawDropLocation.position.x, clawDropLocation.position.z);
    }

    //move claw based on current vector
    private void Update()
    {
        if (CurrentClawMoveDir != Vector2.zero)
        {
            MoveClawBase(CurrentClawMoveDir);
        }
    }

    #region ACTION
    public void UpdateClawMoveDir(Vector2 dir)
    {
        CurrentClawMoveDir = dir;
    }
   
    public void StartDropProcess()
    {
        if (IsClawDropping)
        {
            Debug.LogWarning("Already dropping claw");
            return;
        }

        OnStartDrop?.Invoke();
        clawDropProcess = StartCoroutine(ClawDropPocess());
    }

    private IEnumerator ClawDropPocess()
    {
        //while loops keep running transformation until bound is reached
        while (OpenClaws()) //opne claws
            yield return null;

        while (LowerArm()) //lower arm
            yield return null;

        while (CloseClaws()) //close claws
            yield return null;

        while (RaiseArm()) //raise arm
            yield return null;

        yield return AutoMoveToPosition(dropPos); //move to drop hole

        while (OpenClaws()) //opne claws
            yield return null;

        while (CloseClaws()) //close claws
            yield return null;

        yield return AutoMoveToPosition(startingPos); //move to starting pos

        clawDropProcess = null;
        OnEndDrop?.Invoke();
    }
    #endregion

    #region HELPER
    private bool OpenClaws() => RotateClawLeafs(LeafRotation.Open);

    private bool CloseClaws() => RotateClawLeafs(LeafRotation.Close);

    private bool RaiseArm() => ExtendClawArm(ArmDirection.Up);

    private bool LowerArm() => ExtendClawArm(ArmDirection.Down);

    private IEnumerator AutoMoveToPosition(Vector2 targetPos) //use for going to and returning from prize drop area
    {
        Vector2 moveDir = (targetPos - currentBasePos).normalized;
        while (Vector2.Distance(currentBasePos, targetPos) > 0.1f)
        {
            MoveClawBase(moveDir);
            yield return null;
        }
    }
    #endregion

    #region TRANSFORM
    public void MoveClawBase(Vector2 direction)
    {
        direction *= clawMoveSpeed; //apply speed

        float targetX = currentBasePos.x + direction.x * Time.deltaTime; //calculate value to add to current pos
        float targetZ = currentBasePos.y + direction.y * Time.deltaTime;

        if (targetX < clawBaseMinPos.x) //handle trying to move past bounds with clamp
        {
            targetX = clawBaseMinPos.x;
        }
        else if (targetX > clawBaseMaxPos.x)
        {
            targetX = clawBaseMaxPos.x;
        }

        if (targetZ < clawBaseMinPos.y)
        {
            targetZ = clawBaseMinPos.y;
        }
        else if (targetZ > clawBaseMaxPos.y)
        {
            targetZ = clawBaseMaxPos.y;
        }

        NodeTransformer.TranslateNode(clawBase, targetX, Axis.X);
        NodeTransformer.TranslateNode(clawBase, targetZ, Axis.Z);

        currentBasePos = new Vector2(targetX, targetZ); //update current position
    }

    public bool ExtendClawArm(ArmDirection direction)
    {
        float directionValue =  direction == ArmDirection.Up ? 1 : -1;
        directionValue *= clawExtendSpeed;

        float targetY = currentArmPos + directionValue * Time.deltaTime;

        if (targetY < clawArmExtendMinMax.x) //return false to indicate bound reached and clamp
        {
            targetY = clawArmExtendMinMax.x;
            NodeTransformer.TranslateNode(clawArm, targetY, Axis.Y);
            return false;
        }
        else if (targetY > clawArmExtendMinMax.y)
        {
            targetY = clawArmExtendMinMax.y;
            NodeTransformer.TranslateNode(clawArm, targetY, Axis.Y);
            return false;
        }

        //bounds not reached yet
        NodeTransformer.TranslateNode(clawArm, targetY, Axis.Y); 

        currentArmPos = targetY;
        return true; 
    }

    public bool RotateClawLeafs(LeafRotation direction)
    {
        return (RotateClawLeaf1(direction, clawOpenSpeed) && RotateClawLeaf2(direction, clawOpenSpeed));
    }

    private bool RotateClawLeaf1(LeafRotation direction, float speed)
    {
        float rotValue = direction == LeafRotation.Open ? 1 : -1;
        rotValue *= speed;
        //Debug.Log($"current {currentLeaf1Rot} rotValue {rotValue}");
        float targetRot = currentLeaf1Rot + rotValue * Time.deltaTime;

        if (targetRot < clawLeaf1RotMinMax.x) //reached max or min bound
        {
            targetRot = clawLeaf1RotMinMax.x;
            NodeTransformer.RotateNode(clawLeaf1, targetRot, leaf1OriginalRot, Axis.Z, RotationSpace.Local);
            currentLeaf1Rot = targetRot;
            return false;
        }
        else if (targetRot > clawLeaf1RotMinMax.y)
        {
            targetRot = clawLeaf1RotMinMax.y;
            NodeTransformer.RotateNode(clawLeaf1, targetRot, leaf1OriginalRot, Axis.Z, RotationSpace.Local);
            currentLeaf1Rot = targetRot;
            return false;
        }

        NodeTransformer.RotateNode(clawLeaf1, targetRot, leaf1OriginalRot, Axis.Z, RotationSpace.Local);

        currentLeaf1Rot = targetRot;
        return true;
    }

    private bool RotateClawLeaf2(LeafRotation direction, float speed)
    {
        float rotValue = direction == LeafRotation.Open ? -1 : 1;
        rotValue *= speed;

        float targetRot = currentLeaf2Rot + rotValue * Time.deltaTime;

        if (targetRot < clawLeaf2RotMinMax.x) //reached max or min bound
        {
            targetRot = clawLeaf2RotMinMax.x;
            NodeTransformer.RotateNode(clawLeaf2, targetRot, leaf2OriginalRot, Axis.Z, RotationSpace.Local);
            currentLeaf2Rot = targetRot;
            return false;
        }
        else if (targetRot > clawLeaf2RotMinMax.y)
        {
            targetRot = clawLeaf2RotMinMax.y;
            NodeTransformer.RotateNode(clawLeaf2, targetRot, leaf2OriginalRot, Axis.Z, RotationSpace.Local);
            currentLeaf2Rot = targetRot;
            return false;
        }

        NodeTransformer.RotateNode(clawLeaf2, targetRot, leaf2OriginalRot, Axis.Z, RotationSpace.Local);

        currentLeaf2Rot = targetRot;
        return true;
    }
    #endregion
}
