using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int CurrentScore { get; private set; } = 0;
    public int AttemptsLeft { get; private set; } = 10;

    [SerializeField] private int scoreToWin = 5;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (StaticManager.currentMode == Mode.Challenge)
            ClawController.Instance.OnEndDrop += CheckIfLost;
    }

    public void GotPrize(int scoreValue = 1)
    {
        CurrentScore += scoreValue;
        UIManager.Instance.SetScore(CurrentScore);
        CheckIfWin();
    }

    private void CheckIfWin()
    {
        if (CurrentScore >= scoreToWin)
        {
            TriggerWin();
        }
    }

    private void TriggerWin()
    {
        if (UIManager.Instance.IsPaused)
        {
            UIManager.Instance.OnResumePressed();
        }
        UIManager.Instance.ShowWinPanel();
    }

    public void UseAttempt()
    {
        AttemptsLeft--;
        UIManager.Instance.SetAttempts(AttemptsLeft);
    }

    private bool OutOfAttempts() => AttemptsLeft <= 0;

    private void CheckIfLost()
    {
        StartCoroutine(WaitToCheckIfLost());
    }

    private IEnumerator WaitToCheckIfLost()
    {
        yield return new WaitForSeconds(1);

        if (OutOfAttempts() && CurrentScore < scoreToWin)
        {
            UIManager.Instance.ShowGameOverPanel();
        }
    }
}
