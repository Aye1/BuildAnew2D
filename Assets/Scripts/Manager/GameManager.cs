using UnityEngine;
using System.Collections.Generic;
using System.Linq;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private List<BaseCondition> _successConditions;
    [SerializeField] private List<BaseCondition> _defeatConditions;
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

    public void NextTurn()
    {
        if (_defeatConditions.Any(x => x.IsConditionVerified()))
        {
            TriggerGameOver();
        }
        else if (_successConditions.Count > 0 && _successConditions.All(x => x.IsConditionVerified()))
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