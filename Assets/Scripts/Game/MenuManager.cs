using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    [SerializeField] private GameObject freePlayInfo;
    [SerializeField] private GameObject challengeModeInfo;

    private void Awake()
    {
        Instance = this;
    }

    public void OnFreePlayPressed()
    {
        StaticManager.currentMode = Mode.FreePlay;
        LoadGameScene();
    }

    public void OnChallengeModePressed()
    {
        StaticManager.currentMode = Mode.Challenge;
        LoadGameScene();
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene(StaticManager.GAME_SCENE_INDEX);
    }

    public void HideInfo()
    {
        freePlayInfo.SetActive(false);
        challengeModeInfo.SetActive(false);
    }

    public void ShowFreePlayInfo()
    {
        freePlayInfo.SetActive(true);
        challengeModeInfo.SetActive(false);
    }

    public void ShowChallengeModenfo()
    {
        freePlayInfo.SetActive(false);
        challengeModeInfo.SetActive(true);
    }
}
