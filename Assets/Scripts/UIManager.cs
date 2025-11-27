using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [HideInInspector] public bool AllowClawControl = true;
    private bool upPressed, downPressed, leftPressed, rightPressed;
    private Vector2 clawMoveDir = Vector2.zero;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ClawController.Instance.OnStartDrop += DisableClawControl;
        ClawController.Instance.OnEndDrop += EnableClawControl;
    }

    //update claw movement vector based on buttons pressed
    void Update()
    {
        if (AllowClawControl == false)
            return;

        float x, y;

        if (upPressed) //set z axis
            y = 1;
        else if (downPressed) 
            y = -1;
        else
            y = 0;
        
        if (rightPressed) //set x axis
            x = 1;
        else if (leftPressed)
            x = -1;
        else
            x = 0;

        Vector2 newDir = new Vector2(x, y).normalized;
        if (newDir == clawMoveDir)
            return;

        clawMoveDir = newDir;
        ClawController.Instance.UpdateClawMoveDir(clawMoveDir);
    }

    #region CLAW
    public void OnClawUpPressed() { upPressed = true; }
    public void OnClawUpReleased() { upPressed = false; }

    public void OnClawDownPressed() { downPressed = true; }
    public void OnClawDownReleased() { downPressed = false; }

    public void OnClawLeftPressed() { leftPressed = true; }
    public void OnClawLeftReleased() { leftPressed = false; }

    public void OnClawRightPressed() { rightPressed = true; }
    public void OnClawRightReleased() { rightPressed = false; }

    public void OnClawDropPressed()
    {
        if (AllowClawControl == false)
            return;

        ClawController.Instance.StartDropProcess();
    }

    private void EnableClawControl() => AllowClawControl = true;
    private void DisableClawControl() => AllowClawControl = false;
    #endregion

    #region CAMERA
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
    #endregion
}
