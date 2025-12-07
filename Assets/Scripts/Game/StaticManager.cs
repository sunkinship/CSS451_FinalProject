using UnityEngine;
using UnityEngine.SceneManagement;

public enum Mode { FreePlay, Challenge }

public class StaticManager : MonoBehaviour
{
    public static StaticManager Instance;

    public const int MENU_SCENE_INDEX = 0;
    public const int GAME_SCENE_INDEX = 1;

    public static Mode currentMode = Mode.FreePlay;
    public static int SkyboxIndex = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }

        Instance = this;
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.SetUIToMode();
        }
        if (CameraController.Instance != null)
        {
            CameraController.Instance.SetCameraToMode();
        }
    }
}
