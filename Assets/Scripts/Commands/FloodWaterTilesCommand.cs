using System.Collections.Generic;
using System.Linq;

public class FloodWaterTilesCommand : Command
{
    public override void Execute()
    {
        // Flood individual tiles
        IEnumerable<BaseTileData> waterTiles = TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water);
        waterTiles.ToList().ForEach(x => ((WaterTile)x.terrainTile).IncrementFlood());
    }

    public override string GetDescription()
    {
        return "Flooding all water tiles individually";
    }

    public override void Undo()
    {
        IEnumerable<BaseTileData> waterTiles = TilesDataManager.Instance.GetTilesWithTerrainType(TerrainType.Water);
        waterTiles.ToList().ForEach(x => ((WaterTile)x.terrainTile).DecrementFlood());
    }
}
