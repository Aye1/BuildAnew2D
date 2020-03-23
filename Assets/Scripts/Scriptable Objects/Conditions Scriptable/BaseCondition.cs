using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BaseConditionData", order = 1)]
public abstract class BaseCondition : ScriptableObject
{
    public abstract bool IsConditionVerified();
}