using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public static bool IsGameReady;
    public static bool IsLevelLoaded;

    private LevelData _levelData;

    #region Events
    public delegate void LevelLoaded();
    public static event LevelLoaded OnLevelLoaded;

    public delegate void GameReady();
    public static event GameReady OnGameReady;
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
        RegisterGameReadyCallbacks();
        LevelManager.OnLevelNeedReset += Reset;
        LoadLevel();
        MouseManager.OnPlayerClick += OnPlayerClick;
        CheckIfGameIsReady();
    }

    private void RegisterGameReadyCallbacks()
    {
        TilesDataManager.OnTilesLoaded += CheckIfGameIsReady;
        WaterClusterManager.OnClustersCreated += CheckIfGameIsReady;
    }

    private void UnregisterGameReadyCallbacks()
    {
        TilesDataManager.OnTilesLoaded -= CheckIfGameIsReady;
        WaterClusterManager.OnClustersCreated -= CheckIfGameIsReady;
    }

    private void CheckIfGameIsReady()
    {
        bool gameReady = true;
        gameReady &= IsLevelLoaded;
        gameReady &= TilesDataManager.AreTileLoaded;
        gameReady &= WaterClusterManager.AreClustersCreated;
        if(gameReady)
        {
            IsGameReady = true;
            OnGameReady?.Invoke();
            UnregisterGameReadyCallbacks();
        }
    }

    public LevelData GetLevelData()
    {
        return _levelData;
    }

    public void LoadLevel()
    {
        IsLevelLoaded = false;
        _levelData = LevelManager.Instance.GetCurrentLevel();
        if (_levelData != null)
        {
            ResourcesManager.Instance.InitializeResources(_levelData.GetInitialResources());
        }
        else
        {
            Debug.LogWarning("Missing level data into GameManager");
        }
        IsLevelLoaded = true;
        OnLevelLoaded?.Invoke();
    }

    private void Reset()
    {
        LoadLevel();
        TilesDataManager.Instance.ResetLevel();
        UIManager.Instance.ResetUI();
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