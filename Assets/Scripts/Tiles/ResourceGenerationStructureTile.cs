using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public abstract class ResourceGenerationStructureTile : StructureTile
{
    public ResourceGenerationStrategy strategy;
    public abstract ResourceGenerationStrategy GetGenerationStrategy();
    public int GetResourceAmount()
    {
        return GetStructureData().resourceGenerated.amount;
    }
    public ResourceType GetResourceGeneratedType()
    {
        return GetStructureData().resourceGenerated.type;
    }

    public abstract IEnumerable<ResourceTile> GetNeighbour(IEnumerable<BaseTileData> neighbours);

    protected ResourceGenerationStructureData GetStructureData()
    {
        return (ResourceGenerationStructureData)(structureData);
    }
    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        base.OnTurnStarts(neighbours);
        if (IsOn)
        {
            // Get all wood tiles, from the one with the less resources to the one with the most
            //IEnumerable<BaseTileData> woodTiles = neighbours.Where(x => x.terrainTile is WoodsTile)
            //.OrderBy(x => ((WoodsTile)x.terrainTile).WoodAmount);
            IEnumerable<ResourceTile> neighboursTiles = GetNeighbour(neighbours);
            IEnumerable<ResourceTile> orderedTiles = neighboursTiles.OrderBy(x => x._resourceAmount);
            int resourceAmount = GetGenerationStrategy().GenerateResource(orderedTiles, GetResourceAmount());
            ResourcesManager.Instance.AddResource(new Cost(resourceAmount, GetResourceGeneratedType()));

        }
    }
}
