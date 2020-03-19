
public enum TerrainType { Default, Plains, Water, Wood };

public abstract class TerrainTile : ActiveTile
{
    public TerrainType terrainType;
    public abstract TerrainType GetTerrainType();
    public override void Init()
    {
        terrainType = GetTerrainType();
        base.Init();
    }
    public override string GetText()
    {
        return terrainType.ToString();
    }
}
