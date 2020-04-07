using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGameView : MonoBehaviour
{

    #region Editor Bindings
#pragma warning disable 0649
    [SerializeField] private Button _resetButton;
    [SerializeField] private Button _nextLevelButton;
    [SerializeField] private TextMeshProUGUI _titleText;
#pragma warning restore 0649
    #endregion

    private string wonText = "Victory!";
    private string failedText = "Defeat :(";

    // Start is called before the first frame update
    void Start()
    {
        _resetButton.onClick.AddListener(LevelManager.Instance.ResetCurrentLevel);
        _nextLevelButton.onClick.AddListener(LevelManager.Instance.LoadNextLevel);
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        _titleText.gameObject.SetActive(true);
        if (GameManager.Instance != null)
        {
            GameState gameState = GameManager.Instance.State;
            if (gameState == GameState.Won)
            {
                _resetButton.gameObject.SetActive(false);
                _nextLevelButton.gameObject.SetActive(true);
                _titleText.text = wonText;
            }
            else
            {
                _resetButton.gameObject.SetActive(true);
                _nextLevelButton.gameObject.SetActive(false);
                _titleText.text = failedText;
            }
        }
    }
}
