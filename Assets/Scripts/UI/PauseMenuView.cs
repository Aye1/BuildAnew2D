using UnityEngine;
using UnityEngine.UI;

public class PauseMenuView : MonoBehaviour
{
#pragma warning disable 0649
    [Header("Editor bindings")]
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _closeButton;
#pragma warning restore 0649

    // Start is called before the first frame update
    void Start()
    {
        RegisterListeners();
    }

    private void RegisterListeners()
    {
        _mainMenuButton.onClick.AddListener(GoToMainMenu);
        _restartButton.onClick.AddListener(RestartLevel);
        _settingsButton.onClick.AddListener(OpenSettingsView);
        _closeButton.onClick.AddListener(CloseView);
    }

    public void OpenSettingsView()
	{
        UIManager.Instance.OpenSettingsView();
	}

    public void CloseView()
    {
        Destroy(gameObject);
    }

    public void RestartLevel()
    {
        LevelManager.Instance.ResetCurrentLevel();
        CloseView();
    }

    public void GoToMainMenu()
    {
        // TODO
    }
}
