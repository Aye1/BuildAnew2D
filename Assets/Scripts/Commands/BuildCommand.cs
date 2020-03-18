using UnityEngine;

public class BuildCommand : Command
{
    public Vector3Int position;
    public StructureType type;
    public Cost cost;

    public BuildCommand()
    {
        position = Vector3Int.zero;
        type = StructureType.None;
        cost = Cost.zero;
    }

    public BuildCommand(StructureType type, Vector3Int position)
    {
        this.position = position;
        this.type = type;
        cost = GetCostForStructureType(type);
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

    private Cost GetCostForStructureType(StructureType structure)
    {
        return structure == StructureType.None ? Cost.zero : Cost.debugCost;
    }
}

