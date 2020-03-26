using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private LevelData _levelData;
#pragma warning restore 0649
#endregion

    public LevelData LevelData { get => _levelData;}
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Start()
    {
        LoadLevel();
        MouseManager.OnPlayerClick += OnPlayerClick;
    }

    public void LoadLevel()
    {
        if (_levelData != null)
        {
            ResourcesManager.Instance.Repay(_levelData.GetInitialResources());
        }
        else
        {
            Debug.LogWarning("Missing level data into GameManager");
        }
    }

    public void Reset()
    {
        TilesDataManager.Instance.ResetLevel();
    }

    private void OnPlayerClick()
    {
        ComputeEndGameCondition();
    }

    public void NextTurn()
    {
        TurnManager.Instance.NextTurn();
        ComputeEndGameCondition();        
    }
    private void ComputeEndGameCondition()
    {
        if (_levelData.GetDefeatConditions().Any(x => x.IsConditionVerified()))
        {
            TriggerGameOver();
        }
        else if (_levelData.GetSuccessConditions().Count > 0 && _levelData.GetSuccessConditions().All(x => x.IsConditionVerified()))
        {
            TriggerGameSuccess();
        }
    }

    private void TriggerGameOver()
    {
        UIManager.Instance.TriggerGameOver();
    }
    private void TriggerGameSuccess()
    {
        UIManager.Instance.TriggerGameSuccess();
    }
}