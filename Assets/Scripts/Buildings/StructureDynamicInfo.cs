using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StructureDynamicInfo
{
    public StructureLevel structureLevel = StructureLevel.Level0;
    public List<AbstractModuleScriptable> activeModules = new List<AbstractModuleScriptable>();
    public bool isFloodable = true;

    public void InitDynamicInfo(StructureData staticData)
    {
        isFloodable = staticData.CanStructureBeFlooded();
    }

    public bool CanStructureBeFlooded()
    {
        return isFloodable;
    }
    public void AddModule(AbstractModuleScriptable module)
    {
        activeModules.Add(module);
        module.ApplyModuleEffect(this);
    }
}
