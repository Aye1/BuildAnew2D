using System.Collections.Generic;
using System.Linq;

public class MineTile : ResourceGenerationStructureTile
{
    public override ResourceGenerationStrategy GetGenerationStrategy()
    {
        return new NineNeighboursStrategy();
    }

    public override IEnumerable<ResourceTile> GetNeighbour(IEnumerable<BaseTileData> neighbours)
    {
        IEnumerable<StoneTile> stoneTiles = neighbours.Where(x => x.terrainTile is StoneTile)
                                                          .Select(x => (StoneTile)x.terrainTile);
        return stoneTiles;
    }

    public override ResourceType GetResourceGeneratedType()
    {
        return ResourceType.Stone;
    }

    public override StructureType GetStructureType()
    {
        return StructureType.Mine;
    }
}
