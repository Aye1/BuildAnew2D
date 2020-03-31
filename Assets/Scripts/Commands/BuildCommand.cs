using UnityEngine;
using System.Collections.Generic;

public class BuildCommand : Command
{
    public Vector3Int position;
    public StructureType type;
    public IEnumerable<Cost> cost;

    public BuildCommand()
    {
        position = Vector3Int.zero;
        type = StructureType.None;
        cost = Cost.zeros;
    }

    public BuildCommand(StructureType type, Vector3Int position)
    {
        this.position = position;
        this.type = type;
        cost = TilesDataManager.Instance.CostForStructure(type);
    }

    public override void Execute()
    {
        bool canBuild = TilesDataManager.Instance.CanBuildStructureAtPos(type, position);
        if (canBuild)
        {
            TilesDataManager.Instance.BuildStructureAtPos(type, position);
        }
    }

    public override void Undo()
    {
        TilesDataManager.Instance.RemoveStructureAtPos(position);
    }

    public override string GetDescription()
    {
        return "Building structure " + type.ToString() + " at position " + position.ToString(); 
    }
}

