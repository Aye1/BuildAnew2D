using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Module/InsubmersibleModule", order = 1)]

public class InsubmersibleModule : AbstractModuleScriptable
{
    public override void ApplyModuleEffect(StructureTile tile)
    {
        tile.IsFloodable = true;
    }
}
