using System;

public class WoodsTile : TerrainTile
{
    private int _woodResources = 500;

    public override string GetDebugText() 
    {
        return _woodResources.ToString();
    }

    public void CutWood(int amount)
    {
        _woodResources = Math.Max(_woodResources - amount, 0);
    }
}
