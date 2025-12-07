using TMPro;
using UnityEngine;

public class SkyboxChanger : MonoBehaviour
{
    public static SkyboxChanger Instance;

    public Material[] skyboxes;
    [SerializeField] private TMP_Dropdown skyboxUI;

    private void Awake()
    {
        Instance = this;
        skyboxUI.SetValueWithoutNotify(StaticManager.SkyboxIndex);
        SetSkybox(StaticManager.SkyboxIndex);
    }

    public void SetSkybox(int index)
    {
        if (skyboxes[index] == null)
            return;

        StaticManager.SkyboxIndex = index;
        RenderSettings.skybox = skyboxes[index];
        DynamicGI.UpdateEnvironment();
    }
}