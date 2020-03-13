
public class WoodsTile : TerrainTile
{
    private int _woodResources = 500;

    public override string GetDebugText() 
    {
        return _woodResources.ToString();
    }
}
