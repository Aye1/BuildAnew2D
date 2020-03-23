using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SawmillTile : StructureTile
{
    public Sawmill sawmill;

    [SerializeField] private int _sawingAmount = 100;

    public override StructureType GetStructureType()
    {
        return StructureType.Sawmill;
    }

    public override void OnTurnStarts(IEnumerable<BaseTileData> neighbours)
    {
        base.OnTurnStarts(neighbours);
        if (IsOn)
        {
            // Get all wood tiles, from the one with the less resources to the one with the most
            //IEnumerable<BaseTileData> woodTiles = neighbours.Where(x => x.terrainTile is WoodsTile)
            //.OrderBy(x => ((WoodsTile)x.terrainTile).WoodAmount);
            IEnumerable<WoodsTile> woodsTiles = neighbours.Where(x => x.terrainTile is WoodsTile)
                                                          .Select(x => (WoodsTile)x.terrainTile);
           ResourcesManager.Instance.AddResource(new Cost( CutWoodOnTiles(woodsTiles), ResourceType.Wood));

        }
    }

    // Should not be called directly, but useful to keep it public for unit tests
    public int CutWoodOnTiles(IEnumerable<WoodsTile> tiles)
    {
        IEnumerable<WoodsTile> orderedTilesAsc = tiles.OrderBy(x => x.WoodAmount);

        int woodsTilesCount = tiles.Count();
        if(woodsTilesCount == 0)
        {
            return 0;
        }
        // Split the cut between adjacent woods
        int toCut = _sawingAmount / woodsTilesCount;


        int currentCutWood = 0;
        int currentCutTiles = 0;

        foreach (WoodsTile tile in orderedTilesAsc)
        {
            currentCutWood += tile.CutWood(toCut);
            currentCutTiles++;
            // Adjust the quantity to cut in other tiles, if not the full quantity could be cut before
            if (currentCutTiles < woodsTilesCount)
                toCut = (_sawingAmount - currentCutWood) / (woodsTilesCount - currentCutTiles);
        }
        return currentCutWood;
    }
} 
