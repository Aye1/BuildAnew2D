using System.Collections.Generic;

public static class CostManager
{
    public static List<Cost> CostForStructure(StructureType type)
    {
        return Cost.debugCostList;
    }
}
