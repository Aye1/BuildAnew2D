﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

#pragma warning disable 0649
    [SerializeField] private SettingsView _settingsView;
#pragma warning restore 0649

    public void StartGame()
    {
        LevelManager.Instance.SetLevel(0);
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
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
