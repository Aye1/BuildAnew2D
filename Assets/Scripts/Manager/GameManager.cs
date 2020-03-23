using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private LevelData _levelData;
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
        if(_levelData != null)
        {
            ResourcesManager.Instance.Repay(_levelData.GetInitialResources());
        }
        else
        {
            Debug.LogWarning("Missing level data into GameManager");
        }
    }

    public void NextTurn()
    {
        if (_levelData.GetDefeatConditions().Any(x => x.IsConditionVerified()))
        {
            TriggerGameOver();
        }
        else if (_levelData.GetSuccessConditions().Count > 0 && _levelData.GetSuccessConditions().All(x => x.IsConditionVerified()))
        {
            TriggerGameSuccess();
        }
        else
        {
            TurnManager.Instance.NextTurn();
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