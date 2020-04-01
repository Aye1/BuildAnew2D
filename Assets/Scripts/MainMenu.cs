using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
        LevelManager.Instance.SetLevel(0);
    }

    public void SelectLevel()
    {
        SceneManager.LoadScene("LevelSelectionScene", LoadSceneMode.Single);
    }
}
