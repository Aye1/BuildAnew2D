using System.Collections.Generic;

public enum ResourceType { None, Wood }

[System.Serializable]
public class Cost
{
    public int amount;
    public ResourceType type;
    public static Cost zero = new Cost(0, ResourceType.None);
    public static IEnumerable<Cost> zeros = new List<Cost> { zero };
    public static Cost debugCost = new Cost(50, ResourceType.Wood);
    public static List<Cost> debugCostList = new List<Cost> { debugCost };

    public Cost(int amount, ResourceType type)
    {
        this.type = type;
        this.amount = amount;
    }
}

