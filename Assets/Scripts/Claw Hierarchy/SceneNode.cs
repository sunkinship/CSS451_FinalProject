using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneNode : MonoBehaviour
{
    private Matrix4x4 mCombinedParentXform;

    public Vector3 NodeOrigin = Vector3.zero;
    public List<NodePrimitive> PrimitiveList;
    public List<SceneNode> ChildrenList;

    public Quaternion originalRot { get; private set; }

    protected void Start()
    {
        InitializeSceneNode();
    }

    private void InitializeSceneNode()
    {
        mCombinedParentXform = Matrix4x4.identity;
        originalRot = transform.localRotation;
    }

    // This must be called _BEFORE_ each draw!!  @
    public void CompositeXform(ref Matrix4x4 parentXform)
    {
        Matrix4x4 orgT = Matrix4x4.Translate(NodeOrigin);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);

        mCombinedParentXform = parentXform * orgT * trs;

        // propagate to all children
        foreach (SceneNode child in ChildrenList)
            child.CompositeXform(ref mCombinedParentXform);

        // disseminate to primitives
        foreach (NodePrimitive p in PrimitiveList)
            p.LoadShaderMatrix(ref mCombinedParentXform);
    }
}