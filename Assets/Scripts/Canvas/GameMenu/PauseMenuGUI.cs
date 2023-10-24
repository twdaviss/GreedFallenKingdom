using UnityEngine;
using UnityEngine.UI;

public class PauseMenuGUI : SingletonMonobehaviour<PauseMenuGUI>
{
    [SerializeField] private GameObject content = default;

    [Header("Menu Content:")]
    [SerializeField] private Button pm_OptionsMenuButton = default;
    [SerializeField] private Button pm_AbandonRunButton = default;
    [SerializeField] private Button pm_MainMenuButton = default;

    private GameState priorGameState = default;

    //===========================================================================
    private void OnEnable()
    {
        // Pause Menu
        pm_OptionsMenuButton.onClick.AddListener(() =>
        {
            OptionMenuGUI.Instance.SetActive(true);
            SceneControlManager.Instance.GameState = GameState.OptionMenu;

            SetActive(false);
        });

        pm_AbandonRunButton.onClick.AddListener(() => SceneControlManager.Instance.RespawnPlayerAtHub());
        pm_MainMenuButton.onClick.AddListener(() => SceneControlManager.Instance.BackToMainMenuWrapper());
    }

    private void Update()
    {
        if (SceneControlManager.Instance.IsLoadingScreenActive)
            return;

        if (SceneControlManager.Instance.GameState == GameState.MainMenu)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (SceneControlManager.Instance.GameState == GameState.PauseMenu)
            {
                SetActive(false);
                SceneControlManager.Instance.GameState = priorGameState;
                return;
            }

            SetActive(true);
            priorGameState = SceneControlManager.Instance.GameState;
            SceneControlManager.Instance.GameState = GameState.PauseMenu;
        }
    }

    //===========================================================================
    public void SetActive(bool active)
    {
        content.SetActive(active);
    }
}