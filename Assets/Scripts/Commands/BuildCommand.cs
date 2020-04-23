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
        cost = BuildingManager.Instance.StructuresTemplates.CostForStructure(type);
    }

    public override void Execute()
    {
        bool canBuild = BuildingManager.Instance.CanBuildStructureAtPos(type, position);
        if (canBuild)
        {
            BuildingManager.Instance.BuildStructureAtPos(type, position);
        }
    }

    public override void Undo()
    {
        BuildingManager.Instance.RemoveStructureAtPos(position);
    }

    public override string GetDescription()
    {
        return "Building structure " + type.ToString() + " at position " + position.ToString(); 
    }
}

