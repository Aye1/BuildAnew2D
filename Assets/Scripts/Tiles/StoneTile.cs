using System;

public class StoneTile : ResourceTile
{
    public int StoneAmount()
    {
        return _resourceAmount;
    }

    public override TerrainType GetTerrainType()
    {
        return TerrainType.Stone;
    }

    public override void Init()
    {
        _resourceType = ResourceType.Stone;
        _resourceAmount = Alea.GetInt(2, 5);
        base.Init();
    }
}
