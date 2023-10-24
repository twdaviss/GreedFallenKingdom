using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneControlManager : SingletonMonobehaviour<SceneControlManager>
{
    [SerializeField] private CanvasGroup loadingScreenCanvasGroup;
    [SerializeField] private Image loadingScreenImage;

    [SerializeField] private GameObject player;

    [Header("Starting Scene:")]
    [SerializeField] private SceneName startingScene;
    [SerializeField] private Transform startingPosition;

    [Header("Pause Menu:")]
    [SerializeField] private PauseMenuGUI pauseMenu;

    [Header("Options Menu:")]
    [SerializeField] private GameObject optionsMenu;

    [Header("Gameover Menu")]
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private Button gv_respawnButton;
    [SerializeField] private Button gv_mainMenuButton;

    private readonly float loadingScreenDuration = 0.75f;

    private bool isLoadingScreenActive = default;
    public bool IsLoadingScreenActive => isLoadingScreenActive;

    private bool isLoadingScene = default;
    public bool IsLoadingScene => isLoadingScene;

    public GameState GameState = default;

    //===========================================================================
    private void OnEnable()
    {
        // Gameover Menu
        gv_mainMenuButton.onClick.AddListener(() => StartCoroutine(BackToMainMenu()));
        gv_respawnButton.onClick.AddListener(() => StartCoroutine(UnloadAndSwitchScene(SceneName.DemoSceneHub.ToString(), Vector3.zero)));
    }

    private void Start()
    {
        // First Time Load
        loadingScreenImage.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        loadingScreenCanvasGroup.alpha = 1.0f;

        MainMenuGUI.Instance.SetActive(true);

        StartCoroutine(LoadingScreen(0.0f));
    }

    //===========================================================================
    private IEnumerator UnloadAndSwitchScene(string sceneName, Vector3 spawnPosition)
    {
        EventManager.CallBeforeSceneUnloadLoadingScreenEvent();
        yield return StartCoroutine(LoadingScreen(1.0f));

        gameOverMenu.SetActive(false);

        EventManager.CallBeforeSceneUnloadEvent();
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));
        EventManager.CallAfterSceneLoadEvent();

        Player.Instance.transform.position = spawnPosition;
        Player.Instance.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(1.0f);
        yield return StartCoroutine(LoadingScreen(0.0f));
        EventManager.CallAfterSceneLoadedLoadingScreenEvent();

        isLoadingScene = false;
    }

    //===========================================================================
    private IEnumerator LoadingScreen(float targetAlpha)
    {
        isLoadingScreenActive = true;

        loadingScreenCanvasGroup.blocksRaycasts = true;

        float _loadSpeed = Mathf.Abs(loadingScreenCanvasGroup.alpha - targetAlpha) / loadingScreenDuration;

        while (Mathf.Approximately(loadingScreenCanvasGroup.alpha, targetAlpha) == false)
        {
            loadingScreenCanvasGroup.alpha = Mathf.MoveTowards(loadingScreenCanvasGroup.alpha, targetAlpha, _loadSpeed * Time.deltaTime);
            yield return null;
        }

        isLoadingScreenActive = false;
        loadingScreenCanvasGroup.blocksRaycasts = false;
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    //===========================================================================
    private IEnumerator LoadStartingScene()
    {
        EventManager.CallBeforeSceneUnloadLoadingScreenEvent();
        yield return StartCoroutine(LoadingScreen(1.0f));

        MainMenuGUI.Instance.SetActive(false);
        gameOverMenu.SetActive(false);

        yield return StartCoroutine(LoadSceneAndSetActive(startingScene.ToString()));
        EventManager.CallAfterSceneLoadEvent();

        Player.Instance.transform.position = startingPosition.position;
        Player.Instance.gameObject.SetActive(true);

        StartCoroutine(LoadingScreen(0.0f));
        EventManager.CallAfterSceneLoadedLoadingScreenEvent();

        GameState = GameState.Hub;
    }

    private IEnumerator BackToMainMenu()
    {
        EventManager.CallBeforeSceneUnloadLoadingScreenEvent();
        yield return StartCoroutine(LoadingScreen(1.0f));

        MainMenuGUI.Instance.SetActive(true);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);

        EventManager.CallBeforeSceneUnloadEvent();
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        yield return StartCoroutine(LoadingScreen(0.0f));
        EventManager.CallAfterSceneLoadedLoadingScreenEvent();

        GameState = GameState.MainMenu;
    }

    //===========================================================================
    public void LoadStartingSceneWrapper()
    {
        if (isLoadingScreenActive == false)
        {
            StartCoroutine(LoadStartingScene());
        }
    }

    public void BackToMainMenuWrapper()
    {
        if (isLoadingScreenActive == false)
        {
            StartCoroutine(BackToMainMenu());
        }
    }

    public void RespawnPlayerAtHub()
    {
        if (isLoadingScreenActive == false)
        {
            MainMenuGUI.Instance.SetActive(false);
            pauseMenu.SetActive(false);
            gameOverMenu.SetActive(false);

            SaveDataManager.Instance.LoadPlayerDataToRuntimeData(SaveDataSlot.save01);

            StartCoroutine(UnloadAndSwitchScene(SceneName.DemoSceneHub.ToString(), Vector3.zero));
        }
    }

    public void LoadScene(string sceneName, Vector3 spawnPosition)
    {
        if (isLoadingScreenActive == false)
        {
            isLoadingScene = true;

            StartCoroutine(UnloadAndSwitchScene(sceneName, spawnPosition));

            if (sceneName == SceneName.DemoSceneDungeon.ToString())
            {
                GameState = GameState.Dungeon;
            }
        }
    }
}