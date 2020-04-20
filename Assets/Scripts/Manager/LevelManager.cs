using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Dependecies to other managers:
// None

[System.Serializable]
public class LevelBinding
{
    public LevelData levelData;
}

public class LevelManager : Manager
{
    public static LevelManager Instance { get; private set; }

    #region Editor objects
#pragma warning disable 0649
    [SerializeField] private List<LevelBinding> _levelsData;
#pragma warning restore 0649
    #endregion

    #region Events
    public delegate void LevelReset();
    public static event LevelReset OnLevelNeedReset;
    #endregion

    private int _currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitState = InitializationState.Ready;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // TODO: remove getter?
    public List<LevelBinding> GetLevelBindings()
    {
        return _levelsData;
    }

    public void SetLevel(int index)
    {
        _currentLevelIndex = index;
    }

    public void LoadLevel(int index)
    {
        // Changing the init state is a bit overkill, yes, but it triggers events which could be useful
        InitState = InitializationState.Initializing;
        _currentLevelIndex = index;
        OnLevelNeedReset?.Invoke();
        InitState = InitializationState.Ready;
    }

    public void ResetCurrentLevel()
    {
        LoadLevel(_currentLevelIndex);
    }

    public void LoadNextLevel()
    {
        int nextIndex = (_currentLevelIndex + 1) % _levelsData.Count;
        LoadLevel(nextIndex);
    }

    public LevelData GetCurrentLevel()
    {
        return _levelsData[_currentLevelIndex].levelData;
    }
}
