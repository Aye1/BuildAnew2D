
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractModuleScriptable : ScriptableObject
{
    public List<Cost> moduleCosts;
    public Sprite spriteOff;
    public Sprite spriteOn;
    public abstract void ApplyModuleEffect(StructureDynamicInfo dynamicInfo);
}
