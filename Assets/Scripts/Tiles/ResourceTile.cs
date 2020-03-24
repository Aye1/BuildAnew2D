using System;

public abstract class ResourceTile : TerrainTile
{
    public int _resourceAmount;
    public ResourceType _resourceType;

    public override string GetDebugText()
    {
        return _resourceAmount.ToString();
    }

    // Returns the real amount of wood cut
    public int CutResource(int amount)
    {
        int realCutAmount = amount <= _resourceAmount ? amount : _resourceAmount;
        _resourceAmount -= realCutAmount;
        return realCutAmount;
    }
}
