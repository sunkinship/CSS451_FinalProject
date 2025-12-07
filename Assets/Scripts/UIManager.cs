using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [HideInInspector] public bool AllowClawControl = true;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI attemptsText;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject cameraPanel;
    [SerializeField] private GameObject pausePanel;

    private bool upPressed, downPressed, leftPressed, rightPressed;
    private Vector2 clawMoveDir = Vector2.zero;

    private bool leftRotPressed, rightRotPressed;
    private BodyRotation clawRotDor = BodyRotation.None;

    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        SetUIToMode();

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

    public void SetAttempts(int attempts)
    {
        attemptsText.text = attempts.ToString();
    }

    public void ShowWinPanel()
    {
        winPanel.SetActive(true);
    }

    public void ShowGameOverPanel()
    {
        gameOverPanel.SetActive(true);
    }

    public void OnPlayAgainPressed()
    {
        SceneManager.LoadScene(StaticManager.GAME_SCENE_INDEX);
    }

    public void OnQuitPressed()
    {
        OnResumePressed();
        SceneManager.LoadScene(StaticManager.MENU_SCENE_INDEX);
    }

    public void SetUIToMode()
    {
        if (StaticManager.currentMode == Mode.FreePlay)
        {
            attemptsText.transform.parent.gameObject.SetActive(false);
            cameraPanel.SetActive(true);
        } 
        else if (StaticManager.currentMode == Mode.Challenge)
        {
            attemptsText.transform.parent.gameObject.SetActive(true);
            cameraPanel.SetActive(false);
        }
    }
    #endregion

    #region PAUSE
    public void OnPauseToggle()
    {
        IsPaused = !IsPaused;

        if (IsPaused)
        {
            if (winPanel.activeInHierarchy)
                return;

            IsPaused = true;
            pausePanel.SetActive(true);
            Time.timeScale = 0;
        }
        else
            OnResumePressed();
    }

    public void OnResumePressed()
    {
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        IsPaused = false;
    }
    #endregion
}
