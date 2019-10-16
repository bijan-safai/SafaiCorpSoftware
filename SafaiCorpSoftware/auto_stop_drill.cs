private List<IMyCargoContainer> Containers; 
private List<IMyShipDrill> Drills;
private const string CoantainerGroupName = "contgroup";
private const string DrillGroupName = "ID_Drills";
private const int MaxMass = 1640000;

public Program()

{
    //get cotnainers
    IMyBlockGroup contGroup = GridTerminalSystem.GetBlockGroupWithName(CoantainerGroupName);
    Containers = new List<IMyCargoContainer>();
    contGroup.GetBlocksOfType<IMyCargoContainer>(Containers);

    //get drills
    IMyBlockGroup drillGroup = GridTerminalSystem.GetBlockGroupWithName(DrillGroupName);
    Drills = new List<IMyShipDrill>();
    drillGroup.GetBlocksOfType<IMyShipDrill>(Drills);

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}


public void Main(string argument, UpdateType updateSource)

{
    if(ContainersAreFull())
    {
        foreach(IMyShipDrill drill in Drills)
        {
            drill.Enabled = false;
        }
    }
}

public bool ContainersAreFull()
{
    int currMass = 0;
    foreach (IMyCargoContainer container in Containers)
    {
        
        currMass += container.GetInventory().CurrentMass.ToIntSafe();
        if(currMass > MaxMass)
        {
            return true;
        }
    }
    return false;
}