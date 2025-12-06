using System;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public static SkyboxChanger Instance;

    public Material[] skyboxes;

    private void Awake()
    {
        Instance = this;
    }

    public void SetSkybox(int index)
    {
        if (skyboxes[index] == null)
            return;

        RenderSettings.skybox = skyboxes[index];
        DynamicGI.UpdateEnvironment();
    }
}