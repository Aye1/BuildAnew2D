using UnityEngine;
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
            IEnumerable<BaseTileData> woodTiles = neighbours.Where(x => x.terrainTile is WoodsTile);
            int woodsTilesCount = woodTiles.Count();
            // Split the cut between adjacent woods
            int toCut = _sawingAmount / woodsTilesCount;

            // The remaining "cut amount" is for the last of the list
            int resToCut = _sawingAmount % woodsTilesCount;

            foreach (BaseTileData tile in woodTiles)
            {
                ((WoodsTile)tile.terrainTile).CutWood(toCut);
            }

            ((WoodsTile)woodTiles.Last().terrainTile).CutWood(resToCut);
        }
    }
}
