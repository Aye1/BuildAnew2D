using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    [SerializeField] private List<BaseCondition> _successConditions;
    [SerializeField] private List<BaseCondition> _defeatConditions;
    [SerializeField] private List<Cost> _initialResources;

    public List<BaseCondition> GetSuccessConditions()
    {
        return _successConditions;
    }
    public List<BaseCondition> GetDefeatConditions()
    {
        return _defeatConditions;
    }
    public List<Cost> GetInitialResources()
    {
        return _initialResources;
    }
}
