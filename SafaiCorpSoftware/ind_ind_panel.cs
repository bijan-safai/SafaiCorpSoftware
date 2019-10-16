private IMyInteriorLight IndHealthLight;
private IMyInventory RootInv;
private List<IMyTerminalBlock> IndSystem;
private const string ConveyorSystemBlockGroupName = "main_conveyor_ind";
private const string MasterIndName = "ind_health";

public Program()
{
    IMyBlockGroup ConveyorSystemGroup = GridTerminalSystem.GetBlockGroupWithName(ConveyorSystemBlockGroupName);
    IndSystem = new List<IMyTerminalBlock>();
    ConveyorSystemGroup.GetBlocks(IndSystem);
    RootInv = (GridTerminalSystem.GetBlockWithName("root") as IMyTerminalBlock).GetInventory();
    IndHealthLight = GridTerminalSystem.GetBlockWithName(MasterIndName) as IMyInteriorLight;
    IndHealthLight.Color = Color.Green;

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}


public void UpdateStatus(String sourceName, IMyInteriorLight ind)
{
    Color indColor = new Color(0f, 0f, 0f);
    float blinkRate = 0f;
    try
    {
        IMyTerminalBlock source = GridTerminalSystem.GetBlockWithName(sourceName) as IMyTerminalBlock;

        if (source.HasInventory && !RootInv.IsConnectedTo(source.GetInventory()))
        {
            indColor = Color.Red;
            blinkRate = 0.5f;
        }
        else if (!source.IsFunctional)
        {
            indColor = Color.Red;
        }
        else if (!source.IsWorking)
        {
            indColor = Color.Orange;
        }
        else
        {
            indColor = Color.Green;
        }
    }
    catch (System.Exception)
    {
        indColor = Color.Purple;
        blinkRate = 0.5f;
    }

    ind.Color = indColor;  
    ind.BlinkIntervalSeconds = blinkRate;
}


public void Main(string argument, UpdateType updateSource)
{

    foreach(IMyTerminalBlock ind in IndSystem)  
    {
        try
        {
            UpdateStatus(ind.DisplayNameText.Split('_')[0], ind as IMyInteriorLight);
        }
        catch (System.Exception)
        {
            
           IndHealthLight.Color = Color.Red;
        }
    }
}