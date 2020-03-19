
public enum TerrainType { Plains, Water };

public class TerrainTile : ActiveTile
{
    public TerrainType terrainType;
    public override string GetText()
    {
        return terrainType.ToString();
    }
}
