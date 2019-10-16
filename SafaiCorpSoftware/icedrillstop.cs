List<IMyCargoContainer> CargoHolds;
List<IMyPistonBase> Pistons;
List<IMyShipDrill> Drills;

private IMyTextSurface OutPanel;


public Program()

{
    IMyBlockGroup cargoGroup = GridTerminalSystem.GetBlockGroupWithName("cd_Cargo");
    IMyBlockGroup pistonGroup = GridTerminalSystem.GetBlockGroupWithName("cd_Pistons");
    IMyBlockGroup drillGroup = GridTerminalSystem.GetBlockGroupWithName("cd_drills");

    CargoHolds = new List<IMyCargoContainer>();
    Pistons = new List<IMyPistonBase>();
    Drills = new List<IMyShipDrill>();

    cargoGroup.GetBlocksOfType<IMyCargoContainer>(CargoHolds);
    pistonGroup.GetBlocksOfType<IMyPistonBase>(Pistons);
    drillGroup.GetBlocksOfType<IMyShipDrill>(Drills);

    
    IMyProgrammableBlock prog = GridTerminalSystem.GetBlockWithName("drill_prog") as IMyProgrammableBlock;
    OutPanel = prog.GetSurface(0);
}


public void Main(string argument, UpdateType updateSource)
{
    if(argument == "start")
    {
        OutPanel.WriteText("Starting drill...\n", false);
        StartDrill();
    }

    if(CheckFull())
    {
        OutPanel.WriteText("Stopping drill...\n", true);
        StopDrill();
    }
}

public void StopDrill()
{
    foreach (IMyPistonBase piston in Pistons)
    {
        piston.Retract();
    }

    foreach (IMyShipDrill drill in Drills)
    {
        drill.Enabled = false;
    }

    Runtime.UpdateFrequency = 0;
}

public bool CheckFull()
{
    foreach (IMyCargoContainer cargoHold in CargoHolds)
    {
        if(!cargoHold.GetInventory().IsFull)
        {
            return false;
        }
    }
    return true;
}

public void StartDrill()
{
    foreach (IMyPistonBase piston in Pistons)
    {
        piston.Extend();
    }

    foreach (IMyShipDrill drill in Drills)
    {
        drill.Enabled = true;
    }

    Runtime.UpdateFrequency = UpdateFrequency.Update10;
}