using UnityEngine;
using UnityEngine.Tilemaps;

public class BaseTileData
{
    public Vector3Int gridPosition;
    public Vector3 worldPosition;
    public TileBase originTile;

    public TerrainTile terrainTile;
    public StructureTile structureTile;

    public void OnTurnStarts(BaseTileData[] neighbours)
    {
        structureTile?.OnTurnStarts(neighbours);
        terrainTile?.OnTurnStarts(neighbours);
    }

    public void OnTurnEnds(BaseTileData[] neighbours)
    {
        structureTile?.OnTurnEnds(neighbours);
        terrainTile?.OnTurnEnds(neighbours);
    }
}
