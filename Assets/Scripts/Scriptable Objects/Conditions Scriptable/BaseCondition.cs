using UnityEngine;
using System.Collections.Generic;

public abstract class BaseCondition : ScriptableObject
{
    public abstract bool IsConditionVerified();
    public abstract string GetText();
}