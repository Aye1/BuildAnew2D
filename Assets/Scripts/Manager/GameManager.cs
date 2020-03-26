using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private LevelData _levelData;
    #region Events
    public delegate void LevelLoaded();
    public static event LevelLoaded OnLevelLoaded;
    #endregion
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
        LevelManager.OnLevelNeedReset += Reset;
        LoadLevel();
        OnLevelLoaded?.Invoke();
        MouseManager.OnPlayerClick += OnPlayerClick;
    }
    public LevelData GetLevelData()
    {
        return _levelData;
    }

    public void LoadLevel()
    {
        _levelData = LevelManager.Instance.GetCurrentLevel();
        if (_levelData != null)
        {
            ResourcesManager.Instance.InitializeResources(_levelData.GetInitialResources());
        }
        else
        {
            Debug.LogWarning("Missing level data into GameManager");
        }
    }

    public void Reset()
    {
        LoadLevel();
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