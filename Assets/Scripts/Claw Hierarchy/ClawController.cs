using UnityEngine;
using static UnityEditor.FilePathAttribute;

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

    [Header("Claw Transform Bounds")]
    [SerializeField] private Vector2 clawBaseMaxPos; //max and min positions to move on XZ axis
    [SerializeField] private Vector2 clawBaseMinPos;

    [SerializeField] private Vector2 clawArmExtendMinMax; //x is min scale, y is max scale

    [SerializeField] private Vector2 clawLeaf1RotMinMax; //x in min, y is max rotation on axis
    [SerializeField] private Vector2 clawLeaf2RotMinMax;

    //track current transfomrations to be able to clamp
    private Vector2 currentBasePos;
    private float currentArmExtend;
    private float currentLeaf1Rot;
    private float currentLeaf2Rot;

    //track original rotations for rotation calculation
    private Quaternion leaf1OriginalRot;
    private Quaternion leaf2OriginalRot;


    private void Awake()
    {
        Instance = this;
    }

    public void MoveClawBase(Vector2 direction, float speed)
    {
        direction = direction.normalized * speed; //apply speed

        float newX = currentBasePos.x + direction.x * Time.deltaTime; //calculate value to add to current pos
        float newZ = currentBasePos.y + direction.y * Time.deltaTime;

        newX = Mathf.Clamp(newX + currentBasePos.x, clawBaseMinPos.x, clawBaseMaxPos.y); //clamp new value
        newZ = Mathf.Clamp(newX + currentBasePos.x, clawBaseMinPos.y, clawBaseMaxPos.y);      

        NodeTransformer.TranslateNode(clawBase, newX, Axis.X);
        NodeTransformer.TranslateNode(clawBase, newZ, Axis.Z);

        currentBasePos = new Vector2(newX, newZ); //update current position
    }

    public void ExtendClawArm(ArmDirection direction, float speed)
    {
        float directionValue =  direction == ArmDirection.Up ? 1 : -1;
        directionValue *= speed;

        float newScale = currentArmExtend + directionValue * Time.deltaTime;

        newScale = Mathf.Clamp(newScale, clawArmExtendMinMax.x, clawArmExtendMinMax.y);

        NodeTransformer.ScaleNode(clawArm, newScale, Axis.Y);

        currentArmExtend = newScale;
    }

    public void RotateClawLeafs(LeafRotation direction, float speed)
    {
        RotateClawLeaf1(direction, speed);
        RotateClawLeaf2(direction, speed);
    }

    private void RotateClawLeaf1(LeafRotation direction, float speed)
    {
        float rotValue = direction == LeafRotation.Open ? -1 : 1;
        rotValue *= speed;

        float newRot = currentLeaf1Rot + rotValue * Time.deltaTime;

        newRot = Mathf.Clamp(newRot, clawLeaf1RotMinMax.x, clawLeaf1RotMinMax.y);

        NodeTransformer.RotateNode(clawLeaf1, newRot, leaf1OriginalRot, Axis.X, RotationSpace.Local);

        currentLeaf1Rot = newRot;
    }

    private void RotateClawLeaf2(LeafRotation direction, float speed)
    {
        float rotValue = direction == LeafRotation.Open ? 1 : -1;
        rotValue *= speed;

        float newRot = currentLeaf2Rot + rotValue * Time.deltaTime;

        newRot = Mathf.Clamp(newRot, clawLeaf2RotMinMax.x, clawLeaf2RotMinMax.y);

        NodeTransformer.RotateNode(clawLeaf2, newRot, leaf2OriginalRot, Axis.X, RotationSpace.Local);

        currentLeaf2Rot = newRot;
    }
}
