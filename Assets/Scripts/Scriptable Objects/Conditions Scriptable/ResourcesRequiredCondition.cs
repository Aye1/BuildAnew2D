using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ResourcesRequiredConditionData", order = 1)]

public class ResourcesRequiredCondition : BaseCondition
{
    public List<Cost> _resourcesRequired;

    public override bool IsConditionVerified()
    {
       return ResourcesManager.Instance.CanPay(_resourcesRequired);
    }
}
