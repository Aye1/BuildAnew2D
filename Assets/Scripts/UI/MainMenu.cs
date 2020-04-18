using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

#pragma warning disable 0649
    [SerializeField] private SettingsView _settingsView;
#pragma warning restore 0649

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        LevelManager.Instance.SetLevel(0);
    }

    public void SelectLevel()
    {
        SceneManager.LoadScene("LevelSelectionScene", LoadSceneMode.Single);
    }

    public void OpenSettingsView()
    {
        SettingsView view = Instantiate(_settingsView, Vector3.zero, Quaternion.identity, transform);
        view.transform.localPosition = Vector3.zero;
    }
}
