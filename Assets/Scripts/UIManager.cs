using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [HideInInspector] public bool AllowClawControl = true;

    [Header("Score and Win UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject winPanel;

    private bool upPressed, downPressed, leftPressed, rightPressed;
    private Vector2 clawMoveDir = Vector2.zero;

    private bool leftRotPressed, rightRotPressed;
    private BodyRotation clawRotDor = BodyRotation.None;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        ClawController.Instance.OnStartDrop += DisableClawControl; //disable claw control during drop sequence
        ClawController.Instance.OnEndDrop += EnableClawControl;
    }

    //update claw movement and rotate direction based on buttons pressed
    void Update()
    {
        if (AllowClawControl == false)
            return;

        //handle move
        HandleClawMove();

        //handle rotation
        HandleClawRotation();
    }

    #region CLAW CONTROL
    private void HandleClawMove()
    {
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
        if (newDir == clawMoveDir) //don't update if state not changed
            return;

        clawMoveDir = newDir;
        ClawController.Instance.UpdateClawMoveDir(clawMoveDir);
    }
    
    private void HandleClawRotation()
    {
        BodyRotation rot;

        if (leftRotPressed)
            rot = BodyRotation.Left;
        else if (rightRotPressed)
            rot = BodyRotation.Right;
        else
            rot = BodyRotation.None;

        if (rot == clawRotDor) //don't update if state not changed
            return;

        clawRotDor = rot;
        ClawController.Instance.UpdateBodyRotDir(clawRotDor);
    }

    private void EnableClawControl() => AllowClawControl = true;
    private void DisableClawControl() => AllowClawControl = false;
    #endregion

    #region CLAW BUTTONS
    public void OnClawUpPressed() { upPressed = true; } //move buttons
    public void OnClawUpReleased() { upPressed = false; }

    public void OnClawDownPressed() { downPressed = true; }
    public void OnClawDownReleased() { downPressed = false; }

    public void OnClawLeftPressed() { leftPressed = true; }
    public void OnClawLeftReleased() { leftPressed = false; }

    public void OnClawRightPressed() { rightPressed = true; }
    public void OnClawRightReleased() { rightPressed = false; }


    public void OnClawRotateLeftPressed() { leftRotPressed = true; } //rotate buttons
    public void OnClawRotateLeftReleased() { leftRotPressed = false; }

    public void OnClawRotateRightPressed() { rightRotPressed = true; }
    public void OnClawRotateRightReleased() { rightRotPressed = false; }


    public void OnClawDropPressed() //drop button
    {
        if (AllowClawControl == false)
            return;

        ClawController.Instance.StartDropProcess();
    }

    
    #endregion

    #region CAMERA
    public void OnClawCamToggle()
    {
        CameraController.Instance.ToggleClawMiniCam();
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
    #endregion

    #region GAME
    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }

    public void OnPlayAgainPressed()
    {
        GameManager.Instance.ResetGame();
    }

    public void OnQuitPressed()
    {
        GameManager.Instance.QuitGame();
    }
    #endregion
}
