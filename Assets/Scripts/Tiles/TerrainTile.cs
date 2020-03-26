
public enum TerrainType { Default, Plains, Water, Wood, Stone, Sand };

public abstract class TerrainTile : ActiveTile
{
    public TerrainType terrainType;
    public abstract TerrainType GetTerrainType();
    public TerrainData terrainData;
    public override void Init()
    {
        terrainType = GetTerrainType();
        terrainData = TilesDataManager.Instance.GetDataForTerrain(terrainType);

        base.Init();
    }
    public override string GetText()
    {
        return terrainType.ToString();
    }
}
