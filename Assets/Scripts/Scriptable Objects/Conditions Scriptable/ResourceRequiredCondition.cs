using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Condition/ResourceRequiredConditionData", order = 1)]

public class ResourceRequiredCondition : BaseCondition
{
    public Cost _resourceRequired;

    public override string GetText()
    {
        ResourceData data = ResourcesManager.Instance.GetResourceDataForType(_resourceRequired.type);
        return "Have " + _resourceRequired.amount +" " + data.resourceName;
    }

    public override bool IsConditionVerified()
    {
       return ResourcesManager.Instance.CanPay(_resourceRequired);
    }
}
