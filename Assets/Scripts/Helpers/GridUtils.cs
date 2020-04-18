using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GridUtils
{
    public static IEnumerable<Vector3Int> GetDirectNeighboursPositions(Vector3Int position)
    {
        IEnumerable<Vector3Int> neighbours = new List<Vector3Int>
        {
            new Vector3Int(position.x - 1, position.y, position.z),
            new Vector3Int(position.x + 1, position.y, position.z),
            new Vector3Int(position.x, position.y - 1, position.z),
            new Vector3Int(position.x, position.y + 1, position.z)
        };
        return neighbours;
    }

    public static List<Vector3Int> GetNeighboursPositionsAtDistance(Vector3Int position, int distance)
    {
        List<Vector3Int> list = new List<Vector3Int>();
        for (int i = -distance; i <= distance; i++)
        {
            for (int j = -distance; j <= distance; j++)
            {
                if (Math.Abs(i) + Math.Abs(j) <= distance)
                {
                    list.Add(new Vector3Int(position.x + i, position.y + j, position.z));
                }
            }
        }
        return list;
    }

    public static List<BaseTileData> GetNeighboursTilesOfRelay(BaseTileData baseTileData/*should be a relay tile */)
    {
        RelayTile relayTile = (RelayTile)(baseTileData.structureTile);
        List<Vector3Int> list = GetNeighboursPositionsAtDistance(baseTileData.GridPosition, relayTile.GetActivationAreaRange());
        List<BaseTileData> neighbour = TilesDataManager.Instance.GetTilesAtPos(list).ToList();
        return neighbour;
    }
}
