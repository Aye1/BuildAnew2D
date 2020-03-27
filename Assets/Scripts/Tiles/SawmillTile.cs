using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SawmillTile : ResourceGenerationStructureTile
{
    public Sawmill sawmill;

    public override ResourceGenerationStrategy GetGenerationStrategy()
    {
        return new NineNeighboursStrategy();
    }
    
    public override IEnumerable<ResourceTile> GetNeighbour(IEnumerable<BaseTileData> neighbours)
    {
        IEnumerable<WoodsTile> woodsTiles = neighbours.Where(x => x.terrainTile is WoodsTile)
                                                          .Select(x => (WoodsTile)x.terrainTile);
        return woodsTiles;
    }
    public override StructureType GetStructureType()
    {
        return StructureType.Sawmill;
    }
} 
