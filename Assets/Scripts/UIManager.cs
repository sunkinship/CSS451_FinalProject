using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void OnTopCamPressed()
    {
        CameraController.Instance.SnapToTopView();
    }

    public void OnLeftCamPressed()
    {
        CameraController.Instance.SnapToLeftView();
    }

    public void OnRightCamPressed()
    {
        CameraController.Instance.SnapToRightView();
    }

    public void OnFrontCamPressed()
    {
        CameraController.Instance.SnapToFrontView();
    }
}
