using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelBinding
{
    public LevelData levelData;
}

public class LevelManager : MonoBehaviour
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
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<LevelBinding> GetLevelBindings()
    {
        return _levelsData;
    }

    public void ResetCurrentLevel()
    {
        OnLevelNeedReset?.Invoke();
    }

    public void SetLevel(int index)
    {
        _currentLevelIndex = index ;
        OnLevelNeedReset?.Invoke();
    }
    public void LoadNextLevel()
    {
        _currentLevelIndex = _currentLevelIndex == 1 ? 0 : 1 ;
        OnLevelNeedReset?.Invoke();
    }

    public LevelData GetCurrentLevel()
    {
        return _levelsData[_currentLevelIndex].levelData;
    }
}
