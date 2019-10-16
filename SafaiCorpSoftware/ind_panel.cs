private IMyInteriorLight IndHealthLight;
private IMyInventory RootInv;
private List<IMyTerminalBlock> ConveyorSystemList;
private const string ConveyorSystemBlockGroupName = "main_conveyor";
private const string MasterIndName = "ind_health";

public Program()
{
    IMyBlockGroup ConveyorSystemGroup = GridTerminalSystem.GetBlockGroupWithName(ConveyorSystemBlockGroupName);
    ConveyorSystemList = new List<IMyTerminalBlock>();
    ConveyorSystemGroup.GetBlocks(ConveyorSystemList);
    RootInv = (GridTerminalSystem.GetBlockWithName("root") as IMyTerminalBlock).GetInventory();
    IndHealthLight = GridTerminalSystem.GetBlockWithName(MasterIndName) as IMyInteriorLight;
    IndHealthLight.Color = Color.Green;

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}


public void UpdateStatus(IMyTerminalBlock source, IMyInteriorLight ind)
{
    Color indColor = new Color(0f, 0f, 0f);
    float blinkRate = 0f;
    try
    {
        GridTerminalSystem.GetBlockWithName(source.DisplayNameText);
        if (source.IsWorking)
        {
            indColor = Color.Green;
        }
        else if (source.IsFunctional)
        {
            indColor = Color.Orange;
        }
        else if((source.HasInventory && RootInv.CanTransferItemTo(source.GetInventory(), new MyItemType("Ingot", "Iron"))) || !source.HasInventory)
        {
            indColor = Color.Red;
        }
        else
        {
            indColor = Color.Red;
            blinkRate = 2f;
        }
    }
    catch (System.Exception)
    {
        Echo("CaughtException");
        indColor = Color.Red;
        blinkRate = 0.5f;
    }

    ind.Color = indColor;
    ind.BlinkIntervalSeconds = blinkRate;
}


public void Main(string argument, UpdateType updateSource)
{

    foreach(IMyTerminalBlock block in ConveyorSystemList)  
    {
        try
        {
           IMyInteriorLight ind = GridTerminalSystem.GetBlockWithName(block.DisplayNameText + "_ind") as IMyInteriorLight;
            UpdateStatus(block, ind);
        }
        catch (System.Exception)
        {
            
           IndHealthLight.Color = Color.Red;
        }
    }
}