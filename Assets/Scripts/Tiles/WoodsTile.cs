using System;

public class WoodsTile : ResourceTile
{
    public int WoodAmount()
    {
        return _resourceAmount;
    }

    public override TerrainType GetTerrainType()
    {
        return TerrainType.Wood;
    }

    public override void Init()
    {
        _resourceType = ResourceType.Wood;
        _resourceAmount = Alea.GetInt(300, 500);
        base.Init();
    }
}
