using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int CurrentScore { get; private set; } = 0;

    [SerializeField] private int scoreToWin = 5;

    private void Awake()
    {
        Instance = this;
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
        UIManager.Instance.ShowWinPanel();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
