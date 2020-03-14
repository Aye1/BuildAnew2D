using System;

public class WoodsTile : TerrainTile
{
    public int WoodAmount { get; set; }

    public override void Init()
    {
        base.Init();
        WoodAmount = Alea.GetInt(300, 500);
    }

    public override string GetDebugText() 
    {
        return WoodAmount.ToString();
    }


    // Returns the real amount of wood cut
    public int CutWood(int amount)
    {
        int realCutAmount = amount <= WoodAmount ? amount : WoodAmount;
        WoodAmount -= realCutAmount;
        return realCutAmount;
    }
}
