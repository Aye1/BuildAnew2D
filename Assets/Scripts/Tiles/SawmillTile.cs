﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SawmillTile : StructureTile
{
    public Sawmill sawmill;

    [SerializeField] private int _sawingAmount = 100;

    public override void Init()
    {
        base.Init();
        structureType = StructureType.Sawmill;
    }

    public override void OnTurnStarts(BaseTileData[] neighbours)
    {
        base.OnTurnStarts(neighbours);
        if (sawmill != null && sawmill.IsON)
        {
            // Get all wood tiles, from the one with the less resources to the one with the most
            IEnumerable<BaseTileData> woodTiles = neighbours.Where(x => x.terrainTile is WoodsTile)
            .OrderBy(x => ((WoodsTile)x.terrainTile).WoodAmount);

            int woodsTilesCount = woodTiles.Count();
            // Split the cut between adjacent woods
            int toCut = _sawingAmount / woodsTilesCount;


            int currentCutWood = 0;
            int currentCutTiles = 0;

            foreach (BaseTileData tile in woodTiles)
            {
                currentCutWood += ((WoodsTile)tile.terrainTile).CutWood(toCut);
                currentCutTiles++;
                // Adjust the quantity to cut in other tiles, if not the full quantity could be cut before
                if(currentCutTiles < woodsTilesCount)
                    toCut = (_sawingAmount - currentCutWood) / (woodsTilesCount-currentCutTiles);
            }
            ResourcesManager.Instance.AddWood(currentCutWood);
        }
    }
}
