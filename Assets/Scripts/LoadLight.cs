using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LoadLight : MonoBehaviour
{
    public Transform LightPosition;

    void Update()
    {
        Shader.SetGlobalVector("LightPosition", LightPosition.localPosition);
    }
}
