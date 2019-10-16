private IMyTextPanel OutPanel;

private List<IMyAssembler> Assemblers;
private List<IMyRefinery> Refineries;

private IMyCargoContainer IngotContainer;
private IMyCargoContainer ComponentContainer;

private IMyInventory IngotStore;
private IMyInventory ComponentStore;

private IMyTimerBlock Timer;


private const string IngotStoreName = "Mother Cargo Container 2";
private const string ComponentContainerName = "Mother Cargo Container 1";
private const string OutPanelName = "Text panel 1";
private const string AssemblersGroupName = "MT_Assemblers";
private const string RefineryGroupName = "MT_Refineries";
private const string TimerBlockName = "Black_Timer";

public Program()
{
    OutPanel = GridTerminalSystem.GetBlockWithName(OutPanelName) as IMyTextPanel;

    IMyBlockGroup assemblerGroup = GridTerminalSystem.GetBlockGroupWithName(AssemblersGroupName);
    Assemblers = new List<IMyAssembler>();
    assemblerGroup.GetBlocksOfType<IMyAssembler>(Assemblers);

    IMyBlockGroup refineryGroup = GridTerminalSystem.GetBlockGroupWithName(RefineryGroupName);
    Refineries = new List<IMyRefinery>();
    refineryGroup.GetBlocksOfType<IMyRefinery>(Refineries);

    ComponentContainer = GridTerminalSystem.GetBlockWithName(ComponentContainerName) as IMyCargoContainer;
    IngotContainer = GridTerminalSystem.GetBlockWithName(IngotStoreName) as IMyCargoContainer;

    IngotStore = IngotContainer.GetInventory();
    ComponentStore = ComponentContainer.GetInventory();

    
    Timer = GridTerminalSystem.GetBlockWithName(TimerBlockName) as IMyTimerBlock;
}


public void Main(string argument, UpdateType updateSource)

{
    OutPanel.WriteText("Starting manager...\n", false);
    ClearAssemblers();
    ClearRefineries();
    Timer.StartCountdown();
}

public void ClearAssemblers()
{
    foreach (IMyAssembler assembler in Assemblers)
    {
        if (!assembler.IsProducing)
        {
            OutPanel.WriteText("----------------Clearing Assembler----------------\n", true);
            OutPanel.WriteText("Transfering components ...\n", true);
            TransferAllContents(assembler.OutputInventory, ComponentStore);

            OutPanel.WriteText("Transfering ingots...\n", true);
            TransferAllContents(assembler.InputInventory, IngotStore);
        }
        else
        {
            OutPanel.WriteText("Assembler is producing\n", true);
        }    
    }
}

public void ClearRefineries()
{   
    
    foreach(IMyRefinery refinery in Refineries)
    {
        OutPanel.WriteText("----------------Clearing Refinery----------------\n", true);
        TransferAllContents(refinery.OutputInventory, IngotStore);
    }
}

public void TransferAllContents(IMyInventory source, IMyInventory target)
{
    if(source.IsConnectedTo(target))
    {
        List<MyInventoryItem> items = new List<MyInventoryItem>();
        source.GetItems(items);
        foreach (MyInventoryItem item in items)
        {
            source.TransferItemTo(target, item);
        }
        OutPanel.WriteText("Transfer done.\n", true);
    }
    else
    {
        OutPanel.WriteText("Transfer failed.\n", true);
    }

}