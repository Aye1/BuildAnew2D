using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class NineNeighboursStrategy : ResourceGenerationStrategy
{
    public override int GenerateResource(IEnumerable<ResourceTile> tiles, int resourceAmount)
    {
        int woodsTilesCount = tiles.Count();
        if (woodsTilesCount == 0)
        {
            return 0;
        }
        // Split the cut between adjacent woods
        int toCut = resourceAmount / woodsTilesCount;


        int currentCutWood = 0;
        int currentCutTiles = 0;

        foreach (ResourceTile tile in tiles)
        {
            currentCutWood += tile.CutResource(toCut);
            currentCutTiles++;
            // Adjust the quantity to cut in other tiles, if not the full quantity could be cut before
            if (currentCutTiles < woodsTilesCount)
                toCut = (resourceAmount - currentCutWood) / (woodsTilesCount - currentCutTiles);
        }
        return currentCutWood;
    }
}
