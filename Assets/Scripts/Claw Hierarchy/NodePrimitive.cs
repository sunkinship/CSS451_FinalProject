using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePrimitive : MonoBehaviour
{
    public Color MyColor = new Color(0.1f, 0.1f, 0.2f, 1.0f);
    public Vector3 Pivot;

    private Renderer r;
    private MaterialPropertyBlock material; //used mat property block instead of regular mat to avoid editor error
    public Matrix4x4 LatestWorldMatrix { get; private set; }

    void Awake()
    {
        GetMaterial();
    }

    public void LoadShaderMatrix(ref Matrix4x4 nodeMatrix)
    {
        Matrix4x4 p = Matrix4x4.TRS(Pivot, Quaternion.identity, Vector3.one);
        Matrix4x4 invp = Matrix4x4.TRS(-Pivot, Quaternion.identity, Vector3.one);
        Matrix4x4 trs = Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        Matrix4x4 m = nodeMatrix * p * trs * invp;

        LatestWorldMatrix = m;
        if (r == null || material == null)
            GetMaterial();

        r.GetPropertyBlock(material); //get current mat values

        material.SetMatrix("MyXformMat", m);
        material.SetColor("MyColor", MyColor);

        r.SetPropertyBlock(material); //apply new mat values
    }

    private void GetMaterial()
    {
        r = GetComponent<Renderer>();
        material = new MaterialPropertyBlock();
    }
}