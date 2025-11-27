using UnityEngine;

public enum Axis { X, Y, Z }
public enum RotationSpace { Local, World }

//lower level functions to transform node hierarchy objects
public static class NodeTransformer
{
    public static void TranslateNode(SceneNode node, float pos, Axis axis)
    {
        if (node == null)
            return;

        switch (axis)
        {
            case Axis.X:
                node.NodeOrigin = new Vector3(pos, node.NodeOrigin.y, node.NodeOrigin.z);
                break;
            case Axis.Y:
                node.NodeOrigin = new Vector3(node.NodeOrigin.x, pos, node.NodeOrigin.z);
                break;
            case Axis.Z:
                node.NodeOrigin = new Vector3(node.NodeOrigin.x, node.NodeOrigin.y, pos);
                break;
        }
    }

    public static void RotateNode(SceneNode node, float rot, Quaternion originalRot, Axis axis, RotationSpace space)
    {
        if (node == null)
            return;

        Quaternion q = Quaternion.identity;

        switch (axis)
        {
            case Axis.X:
                q = Quaternion.AngleAxis(rot, Vector3.right);
                break;
            case Axis.Y:
                q = Quaternion.AngleAxis(rot, Vector3.up);
                break;
            case Axis.Z:
                q = Quaternion.AngleAxis(rot, Vector3.forward);
                break;
        }

        if (space == RotationSpace.World)
            node.transform.localRotation = originalRot * q;
        else if (space == RotationSpace.Local)
            node.transform.localRotation = q * originalRot;
    }

    public static void ScaleNode(SceneNode node, float scale, Axis axis)
    {
        if (node == null)
            return;

        switch (axis)
        {
            case Axis.X:
                node.transform.localScale = new Vector3(scale, node.transform.localScale.y, node.transform.localScale.z);
                break;
            case Axis.Y:
                node.transform.localScale = new Vector3(node.transform.localScale.x, scale, node.transform.localScale.z);
                break;
            case Axis.Z:
                node.transform.localScale = new Vector3(node.transform.localScale.x, node.transform.localScale.y, scale);
                break;
        }
    }
}
