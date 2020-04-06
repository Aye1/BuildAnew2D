using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRelayTile : RelayTile
{
    private int _defaultRange = 3;
    
    public override StructureType GetStructureType()
    {
        return StructureType.MainRelay;
    }
    public override int GetActivationAreaRange()
    {
        return _defaultRange;
    }
}
